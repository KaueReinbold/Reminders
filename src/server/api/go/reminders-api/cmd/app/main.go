package main

import (
	"net/http"
	"reminders-go-api/pkg/api"

	"github.com/gin-gonic/gin"
)

func main() {
	repo := api.NewInMemoryReminderRepository()

	handler := api.NewReminderHandler(repo)

	router := gin.Default()

	router.GET("/health", func(c *gin.Context) {
		c.String(http.StatusOK, "Healthy")
	})
	router.GET("/api/reminders", handler.GetReminders)
	router.GET("/api/reminders/count", handler.GetCount)
	router.GET("/api/reminders/:id", handler.GetReminder)
	router.POST("/api/reminders", handler.PostReminder)
	router.PUT("/api/reminders/:id", handler.PutReminder)
	router.DELETE("/api/reminders/:id", handler.DeleteReminder)

	router.Run()
}
