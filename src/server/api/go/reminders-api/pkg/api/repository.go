package api

import (
	"database/sql"
	"errors"
	"log"
	"reminders-go-api/pkg/models"

	"github.com/google/uuid"
	_ "github.com/lib/pq" // Postgres driver
)

var ErrorReminderNotFound = errors.New("reminder not found")

type ReminderRepository interface {
	GetAll() ([]models.Reminder, error)
	Count() (int, error)
	GetByID(id string) (models.Reminder, error)
	Create(reminder models.Reminder) (models.Reminder, error)
	Update(id string, reminder models.Reminder) (models.Reminder, error)
	Delete(id string) error
}

type PostgresReminderRepository struct {
	db *sql.DB

	reminders []models.Reminder
}

func NewPostgresReminderRepository(db *sql.DB) *PostgresReminderRepository {
	return &PostgresReminderRepository{db: db, reminders: []models.Reminder{}}
}

func (repository *PostgresReminderRepository) GetAll() ([]models.Reminder, error) {
	rows, err := repository.db.Query(`
		SELECT 
			"Id", 
			"Title", 
			"Description", 
			"LimitDate", 
			"IsDone"
		FROM "Reminders"
		WHERE
			"IsDeleted" != true
	`)

	if err != nil {
		log.Printf("Error querying database: %v", err)
		return nil, err
	}
	defer rows.Close()

	reminders := []models.Reminder{}

	for rows.Next() {
		reminder := models.Reminder{}

		err = rows.Scan(&reminder.Id, &reminder.Title, &reminder.Description, &reminder.LimitDate, &reminder.IsDone)

		if err != nil {
			log.Printf("Error scanning row: %v", err)
			return nil, err
		}

		reminders = append(reminders, reminder)
	}

	if err = rows.Err(); err != nil {
		log.Printf("Error during rows iteration: %v", err)
		return nil, err
	}

	return reminders, nil
}

func (repository *PostgresReminderRepository) Count() (int, error) {
	row := repository.db.QueryRow(`
		SELECT 
			COUNT(*) 
		FROM "Reminders"
		WHERE
			"IsDeleted" != true
	`)

	count := 0
	err := row.Scan(&count)

	if err != nil {
		log.Fatal(err)
	}

	return count, nil
}

func (repository *PostgresReminderRepository) GetByID(id string) (models.Reminder, error) {
	row := repository.db.QueryRow(`
		SELECT 
			"Id", 
			"Title", 
			"Description", 
			"LimitDate", 
			"IsDone"
		FROM "Reminders"
		WHERE 
			"Id" = $1 AND
			"IsDeleted" != true
	`, id)

	reminder := models.Reminder{}

	err := row.Scan(&reminder.Id, &reminder.Title, &reminder.Description, &reminder.LimitDate, &reminder.IsDone)

	if err != nil {

		if err == sql.ErrNoRows {
			log.Printf("Error scanning row not found: %v", err)
			return models.Reminder{}, ErrorReminderNotFound
		}

		log.Printf("Error scanning row: %v", err)

		return models.Reminder{}, ErrorReminderNotFound
	}

	return reminder, nil
}

func (repository *PostgresReminderRepository) Create(reminder models.Reminder) (models.Reminder, error) {
	reminder.Id = uuid.New().String()
	reminder.IsDeleted = false

	_, err := repository.db.Exec(`
        INSERT INTO "Reminders" (
            "Id",
            "Title",
            "Description",
            "LimitDate",
            "IsDone",
			"IsDeleted"
        ) VALUES (
            $1,
            $2,
            $3,
            $4,
            $5,
			$6
        )
    `, reminder.Id, reminder.Title, reminder.Description, reminder.LimitDate, reminder.IsDone, reminder.IsDeleted)

	if err != nil {
		log.Printf("Error creating new reminder: %v", err)
		return models.Reminder{}, err
	}

	return reminder, nil
}

func (repository *PostgresReminderRepository) Update(id string, reminder models.Reminder) (models.Reminder, error) {
	reminder.Id = id

	_, err := repository.db.Exec(`
        UPDATE "Reminders" SET
            "Title" = $1,
            "Description" = $2,
            "LimitDate" = $3,
            "IsDone" = $4,
            "IsDeleted" = $5
        WHERE 
			"Id" = $6 AND
			"IsDeleted" != true
    `, reminder.Title, reminder.Description, reminder.LimitDate, reminder.IsDone, reminder.IsDeleted, reminder.Id)

	if err != nil {
		log.Printf("Error updating reminder: %v", err)
		return models.Reminder{}, err
	}

	return reminder, nil
}

func (repository *PostgresReminderRepository) Delete(id string) error {
	_, err := repository.db.Exec(`
        UPDATE "Reminders" SET
            "IsDeleted" = $1
        WHERE 
			"Id" = $2;
    `, true, id)

	if err != nil {
		log.Printf("Error deleting reminder: %v", err)
		return err
	}

	return nil
}
