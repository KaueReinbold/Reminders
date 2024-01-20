package main

import (
	"database/sql"
	"log"
	"net/http"
	"os"
	"reminders-go-api/pkg/api"
	"strings"

	"github.com/gin-contrib/cors"
	"github.com/gin-gonic/gin"
	"github.com/joho/godotenv"

	_ "github.com/lib/pq"
)

func main() {
	_ = godotenv.Load()

	connStr := os.Getenv("PostgresDefaultConnection")

	db, err := sql.Open("postgres", connStr)
	if err != nil {
		log.Fatal(err)
	}

	repo := api.NewPostgresReminderRepository(db)

	handler := api.NewReminderHandler(repo)

	router := gin.Default()

	origins := strings.Split(os.Getenv("CorsOrigins"), ",")

	config := cors.DefaultConfig()
	config.AllowOrigins = origins
	config.AllowMethods = []string{"GET", "POST", "PUT", "DELETE"}
	config.AllowHeaders = []string{"Origin", "Content-Length", "Content-Type"}

	router.Use(cors.New(config))

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
