package models

import (
	"encoding/json"
	"errors"
	"time"
)

// ErrInvalidLimitDate reports a limitDate that is neither "YYYY-MM-DD" nor RFC3339.
var ErrInvalidLimitDate = errors.New("limitDate must be a date (YYYY-MM-DD) or an RFC3339 timestamp")

type Reminder struct {
	Id          string    `json:"id"`
	Title       string    `json:"title"`
	Description string    `json:"description"`
	LimitDate   time.Time `json:"limitDate"`
	IsDone      bool      `json:"isDone"`
	IsDeleted   bool      `json:"-"`
}

// ParseLimitDate accepts both "YYYY-MM-DD" (midnight UTC) and RFC3339, and always
// yields UTC, so all three API implementations agree (ADR-0011).
func ParseLimitDate(value string) (time.Time, error) {
	if parsed, err := time.Parse(time.DateOnly, value); err == nil {
		return parsed.UTC(), nil
	}

	if parsed, err := time.Parse(time.RFC3339, value); err == nil {
		return parsed.UTC(), nil
	}

	return time.Time{}, ErrInvalidLimitDate
}

// reminderFields carries every field but limitDate, which is bound as a string so
// both accepted input formats parse and the output is always RFC3339 in UTC.
type reminderFields Reminder

func (reminder *Reminder) UnmarshalJSON(data []byte) error {
	payload := struct {
		LimitDate string `json:"limitDate"`
		*reminderFields
	}{reminderFields: (*reminderFields)(reminder)}

	if err := json.Unmarshal(data, &payload); err != nil {
		return err
	}

	limitDate, err := ParseLimitDate(payload.LimitDate)

	if err != nil {
		return err
	}

	reminder.LimitDate = limitDate

	return nil
}

func (reminder Reminder) MarshalJSON() ([]byte, error) {
	return json.Marshal(struct {
		LimitDate string `json:"limitDate"`
		reminderFields
	}{
		LimitDate:      reminder.LimitDate.UTC().Format(time.RFC3339),
		reminderFields: reminderFields(reminder),
	})
}
