package api

import (
	"errors"
	"net/http"
	"reminders-go-api/pkg/models"

	"github.com/gin-gonic/gin"
)

type ReminderHandler struct {
	repository ReminderRepository
}

func NewReminderHandler(repo ReminderRepository) *ReminderHandler {
	return &ReminderHandler{repository: repo}
}

func (handler *ReminderHandler) GetReminders(c *gin.Context) {
	reminders, err := handler.repository.GetAll()

	if err != nil {
		c.IndentedJSON(http.StatusInternalServerError, gin.H{"message": "Could not get reminders"})
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
		c.IndentedJSON(http.StatusInternalServerError, gin.H{"message": "Could not the count of reminders"})
		return
	}

	c.IndentedJSON(http.StatusOK, count)
}

func (handler *ReminderHandler) GetReminder(c *gin.Context) {
	id := c.Param("id")

	reminder, err := handler.repository.GetByID(id)

	if errors.Is(err, ErrorReminderNotFound) {
		c.IndentedJSON(http.StatusNotFound, gin.H{"message": "Reminder not found"})
		return
	} else if err != nil {
		c.IndentedJSON(http.StatusInternalServerError, gin.H{"message": "Failed to get the reminder"})
		return
	}

	c.IndentedJSON(http.StatusOK, reminder)
}

func (handler *ReminderHandler) PostReminder(c *gin.Context) {
	newReminder := models.Reminder{}

	if err := c.BindJSON(&newReminder); err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{"message": "Invalid body"})
		return
	}

	newReminder, err := handler.repository.Create(newReminder)

	if err != nil {
		c.IndentedJSON(http.StatusInternalServerError, gin.H{"message": "Failed to create the reminder"})
		return
	}

	c.IndentedJSON(http.StatusCreated, newReminder)
}

func (handler *ReminderHandler) PutReminder(c *gin.Context) {
	id := c.Param("id")

	updateReminder := models.Reminder{}

	if err := c.BindJSON(&updateReminder); err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{"message": "Invalid body"})
		return
	}

	reminder, err := handler.repository.Update(id, updateReminder)

	if errors.Is(err, ErrorReminderNotFound) {
		c.IndentedJSON(http.StatusNotFound, gin.H{"message": "Reminder not found"})
		return
	} else if err != nil {
		c.IndentedJSON(http.StatusInternalServerError, gin.H{"message": "Failed to update the reminder"})
		return
	}

	c.IndentedJSON(http.StatusOK, reminder)
}

func (handler *ReminderHandler) DeleteReminder(c *gin.Context) {
	id := c.Param("id")

	err := handler.repository.Delete(id)

	if err != nil {
		c.IndentedJSON(http.StatusInternalServerError, gin.H{"message": "Failed to delete the reminder"})
		return
	}

	c.IndentedJSON(http.StatusOK, id)
}
