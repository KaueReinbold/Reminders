package models

import "time"

type Reminder struct {
	Id          string    `json:"id"`
	Title       string    `json:"title"`
	Description string    `json:"description"`
	LimitDate   time.Time `json:"limitDate"`
	IsDone      bool      `json:"isDone"`
	IsDeleted   bool      `json:"-"`
}
