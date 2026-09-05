package api

import (
	"errors"
	"net/http"
	"reminders-go-api/pkg/models"
	"time"

	"github.com/gin-gonic/gin"
)

// InvalidLimitDate is the message the .NET API already returns, kept identical so
// a client cannot tell which backend answered (ADR-0011).
const InvalidLimitDate = "The Limit Date should be later than Today."

// ReminderNotFound is the .NET message for a missing reminder, kept identical too.
const ReminderNotFound = "Reminder does not exist."

// day is the granularity of the limit date rule: a later day, not a later instant.
const day = 24 * time.Hour

type ReminderHandler struct {
	repository ReminderRepository
}

func NewReminderHandler(repo ReminderRepository) *ReminderHandler {
	return &ReminderHandler{repository: repo}
}

// bindReminder reads the body and reports a binding failure as problem details,
// attributing an unparseable date to the limitDate field.
func bindReminder(c *gin.Context) (models.Reminder, bool) {
	reminder := models.Reminder{}

	err := c.ShouldBindJSON(&reminder)

	if err == nil {
		return reminder, true
	}

	if errors.Is(err, models.ErrInvalidLimitDate) {
		validationProblem(c, map[string][]string{"limitDate": {models.ErrInvalidLimitDate.Error()}})
	} else {
		problem(c, http.StatusBadRequest, "Invalid body")
	}

	return reminder, false
}

func (handler *ReminderHandler) GetReminders(c *gin.Context) {
	reminders, err := handler.repository.GetAll()

	if err != nil {
		problem(c, http.StatusInternalServerError, "Could not get reminders")
		return
	}

	if reminders == nil {
		reminders = []models.Reminder{}
	}

	c.IndentedJSON(http.StatusOK, reminders)
}

func (handler *ReminderHandler) GetCount(c *gin.Context) {
	count, err := handler.repository.Count()

	if err != nil {
		problem(c, http.StatusInternalServerError, "Could not the count of reminders")
		return
	}

	c.IndentedJSON(http.StatusOK, count)
}

func (handler *ReminderHandler) GetReminder(c *gin.Context) {
	id := c.Param("id")

	reminder, err := handler.repository.GetByID(id)

	if errors.Is(err, ErrorReminderNotFound) {
		problem(c, http.StatusNotFound, ReminderNotFound)
		return
	} else if err != nil {
		problem(c, http.StatusInternalServerError, "Failed to get the reminder")
		return
	}

	c.IndentedJSON(http.StatusOK, reminder)
}

func (handler *ReminderHandler) PostReminder(c *gin.Context) {
	newReminder, ok := bindReminder(c)

	if !ok {
		return
	}

	// Create only: an overdue reminder must stay editable, so the update path
	// does not revalidate a date that is already stored (ADR-0011).
	if !newReminder.LimitDate.UTC().Truncate(day).After(time.Now().UTC().Truncate(day)) {
		validationProblem(c, map[string][]string{"limitDate": {InvalidLimitDate}})
		return
	}

	newReminder, err := handler.repository.Create(newReminder)

	if err != nil {
		problem(c, http.StatusInternalServerError, "Failed to create the reminder")
		return
	}

	c.IndentedJSON(http.StatusCreated, newReminder)
}

func (handler *ReminderHandler) PutReminder(c *gin.Context) {
	id := c.Param("id")

	updateReminder, ok := bindReminder(c)

	if !ok {
		return
	}

	reminder, err := handler.repository.Update(id, updateReminder)

	if errors.Is(err, ErrorReminderNotFound) {
		problem(c, http.StatusNotFound, ReminderNotFound)
		return
	} else if err != nil {
		problem(c, http.StatusInternalServerError, "Failed to update the reminder")
		return
	}

	c.IndentedJSON(http.StatusOK, reminder)
}

func (handler *ReminderHandler) DeleteReminder(c *gin.Context) {
	id := c.Param("id")

	err := handler.repository.Delete(id)

	if err != nil {
		problem(c, http.StatusInternalServerError, "Failed to delete the reminder")
		return
	}

	c.IndentedJSON(http.StatusOK, id)
}
