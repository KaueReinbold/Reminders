package main

import (
	"net/http"
	"strconv"
	"time"

	"github.com/gin-gonic/gin"
)

type Reminder struct {
	Id          string    `json:"id"`
	Title       string    `json:"title"`
	Description string    `json:"artist"`
	LimitDate   time.Time `json:"limitDate"`
	IsDone      bool      `json:"isDone"`
}

var reminders = []Reminder{
	{Id: "1", Title: "Reminder 1", Description: "Description 1", LimitDate: time.Date(2022, time.January, 1, 0, 0, 0, 0, time.UTC), IsDone: false},
	{Id: "2", Title: "Reminder 2", Description: "Description 2", LimitDate: time.Date(2022, time.January, 1, 0, 0, 0, 0, time.UTC), IsDone: true},
	{Id: "3", Title: "Reminder 3", Description: "Description 3", LimitDate: time.Date(2022, time.January, 1, 0, 0, 0, 0, time.UTC), IsDone: false},
}

func main() {
	router := gin.Default()

	router.GET("/health", func(c *gin.Context) {
		c.String(http.StatusOK, "Healthy")
	})
	router.GET("/api/reminders", getReminders)
	router.GET("/api/reminders/count", getCount)
	router.GET("/api/reminders/:id", getReminder)
	router.POST("/api/reminders", postReminder)
	router.PUT("/api/reminders/:id", putReminder)
	router.DELETE("/api/reminders/:id", deleteReminder)

	router.Run()
}

func getNextId() string {
	maxId := 0
	for _, reminder := range reminders {
		id, _ := strconv.Atoi(reminder.Id)
		if id > maxId {
			maxId = id
		}
	}
	return strconv.Itoa(maxId + 1)
}

func getReminders(c *gin.Context) {
	c.IndentedJSON(http.StatusOK, reminders)
}

func getCount(c *gin.Context) {
	c.IndentedJSON(http.StatusOK, len(reminders))
}

func getReminder(c *gin.Context) {
	id := c.Param("id")

	for _, reminder := range reminders {
		if reminder.Id == id {
			c.IndentedJSON(http.StatusOK, reminder)
			return
		}
	}

	c.IndentedJSON(http.StatusNotFound, gin.H{"message": "Reminder not found"})
}

func postReminder(c *gin.Context) {
	var newReminder Reminder

	if err := c.BindJSON(&newReminder); err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{"message": "Invalid body"})
		return
	}

	newReminder.Id = getNextId()

	reminders = append(reminders, newReminder)
	c.IndentedJSON(http.StatusCreated, newReminder)
}

func putReminder(c *gin.Context) {
	id := c.Param("id")

	var updateReminder Reminder

	if err := c.BindJSON(&updateReminder); err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{"message": "Invalid body"})
		return
	}

	for index, reminder := range reminders {
		if reminder.Id == id {
			reminders[index] = updateReminder
			c.IndentedJSON(http.StatusOK, updateReminder)
			return
		}
	}

	c.IndentedJSON(http.StatusNotFound, gin.H{"message": "Reminder not found"})
}

func deleteReminder(c *gin.Context) {
	id := c.Param("id")

	for index, reminder := range reminders {
		if reminder.Id == id {
			reminders = append(reminders[:index], reminders[index+1:]...)
			c.IndentedJSON(http.StatusOK, gin.H{"message": "Reminder deleted"})
			return
		}
	}

	c.IndentedJSON(http.StatusNotFound, gin.H{"message": "Reminder not found"})
}
