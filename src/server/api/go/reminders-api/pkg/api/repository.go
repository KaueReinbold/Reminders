package api

import (
	"errors"
	"reminders-go-api/pkg/models"
	"strconv"
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

type InMemoryReminderRepository struct {
	reminders []models.Reminder
}

func getNextId(repository *InMemoryReminderRepository) string {
	maxId := 0
	for _, reminder := range repository.reminders {
		id, _ := strconv.Atoi(reminder.Id)
		if id > maxId {
			maxId = id
		}
	}
	return strconv.Itoa(maxId + 1)
}

func NewInMemoryReminderRepository() *InMemoryReminderRepository {
	return &InMemoryReminderRepository{}
}

func (repository *InMemoryReminderRepository) GetAll() ([]models.Reminder, error) {
	return repository.reminders, nil
}

func (repository *InMemoryReminderRepository) Count() (int, error) {
	return len(repository.reminders), nil
}

func (repository *InMemoryReminderRepository) GetByID(id string) (models.Reminder, error) {
	for _, reminder := range repository.reminders {
		if reminder.Id == id {
			return reminder, nil
		}
	}

	return models.Reminder{}, ErrorReminderNotFound
}

func (repository *InMemoryReminderRepository) Create(reminder models.Reminder) (models.Reminder, error) {
	reminder.Id = getNextId(repository)

	repository.reminders = append(repository.reminders, reminder)

	return reminder, nil
}

func (repository *InMemoryReminderRepository) Update(id string, reminder models.Reminder) (models.Reminder, error) {
	for i, currentReminder := range repository.reminders {
		if currentReminder.Id == id {
			reminder.Id = id
			repository.reminders[i] = reminder
			return reminder, nil
		}
	}

	return models.Reminder{}, ErrorReminderNotFound
}

func (repository *InMemoryReminderRepository) Delete(id string) error {
	for i, reminder := range repository.reminders {
		if reminder.Id == id {
			repository.reminders = append(repository.reminders[:i], repository.reminders[i+1:]...)
			return nil
		}
	}

	return ErrorReminderNotFound
}
