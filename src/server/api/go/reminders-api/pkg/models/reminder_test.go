package models

import (
	"encoding/json"
	"errors"
	"testing"
	"time"
)

func TestParseLimitDate(t *testing.T) {
	midnight := time.Date(2026, 9, 30, 0, 0, 0, 0, time.UTC)

	t.Run("a date only value means midnight UTC", func(t *testing.T) {
		got, err := ParseLimitDate("2026-09-30")
		if err != nil || !got.Equal(midnight) {
			t.Fatalf("got %v err %v", got, err)
		}
	})

	t.Run("an RFC3339 value is normalized to UTC", func(t *testing.T) {
		got, err := ParseLimitDate("2026-09-30T02:00:00+02:00")
		if err != nil || !got.Equal(midnight) || got.Location() != time.UTC {
			t.Fatalf("got %v err %v", got, err)
		}
	})

	t.Run("anything else is ErrInvalidLimitDate", func(t *testing.T) {
		if _, err := ParseLimitDate("30/09/2026"); !errors.Is(err, ErrInvalidLimitDate) {
			t.Fatalf("err = %v", err)
		}
	})
}

func TestReminderJSON(t *testing.T) {
	t.Run("marshals limitDate as RFC3339 in UTC", func(t *testing.T) {
		reminder := Reminder{
			Id:        "id",
			Title:     "Pay rent",
			LimitDate: time.Date(2026, 9, 30, 2, 0, 0, 0, time.FixedZone("east", 2*60*60)),
		}

		body, err := json.Marshal(reminder)
		if err != nil {
			t.Fatal(err)
		}

		var payload map[string]any
		if err := json.Unmarshal(body, &payload); err != nil {
			t.Fatal(err)
		}

		if payload["limitDate"] != "2026-09-30T00:00:00Z" {
			t.Fatalf("limitDate = %v", payload["limitDate"])
		}
		if payload["title"] != "Pay rent" || payload["isDone"] != false {
			t.Fatalf("payload = %+v", payload)
		}
	})

	t.Run("round trips through unmarshal", func(t *testing.T) {
		reminder := Reminder{}
		if err := json.Unmarshal([]byte(`{"title":"T","limitDate":"2026-09-30","isDone":true}`), &reminder); err != nil {
			t.Fatal(err)
		}
		if reminder.Title != "T" || !reminder.IsDone ||
			!reminder.LimitDate.Equal(time.Date(2026, 9, 30, 0, 0, 0, 0, time.UTC)) {
			t.Fatalf("reminder = %+v", reminder)
		}
	})
}
