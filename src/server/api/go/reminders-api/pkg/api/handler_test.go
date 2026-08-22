package api

import (
	"bytes"
	"encoding/json"
	"errors"
	"net/http"
	"net/http/httptest"
	"reminders-go-api/pkg/models"
	"testing"
	"time"

	"github.com/gin-gonic/gin"
)

// fakeRepository lets each test script the repository behaviour.
type fakeRepository struct {
	getAll  func() ([]models.Reminder, error)
	count   func() (int, error)
	getByID func(id string) (models.Reminder, error)
	create  func(r models.Reminder) (models.Reminder, error)
	update  func(id string, r models.Reminder) (models.Reminder, error)
	delete  func(id string) error
}

func (f *fakeRepository) GetAll() ([]models.Reminder, error)         { return f.getAll() }
func (f *fakeRepository) Count() (int, error)                        { return f.count() }
func (f *fakeRepository) GetByID(id string) (models.Reminder, error) { return f.getByID(id) }
func (f *fakeRepository) Create(r models.Reminder) (models.Reminder, error) {
	return f.create(r)
}
func (f *fakeRepository) Update(id string, r models.Reminder) (models.Reminder, error) {
	return f.update(id, r)
}
func (f *fakeRepository) Delete(id string) error { return f.delete(id) }

var sample = models.Reminder{
	Id:          "6f1d2c3e-0000-4000-8000-000000000001",
	Title:       "Pay rent",
	Description: "Transfer before noon",
	LimitDate:   time.Date(2026, 9, 1, 0, 0, 0, 0, time.UTC),
	IsDone:      false,
}

func newRouter(repo ReminderRepository) *gin.Engine {
	gin.SetMode(gin.TestMode)
	handler := NewReminderHandler(repo)
	router := gin.New()
	router.GET("/api/reminders", handler.GetReminders)
	router.GET("/api/reminders/count", handler.GetCount)
	router.GET("/api/reminders/:id", handler.GetReminder)
	router.POST("/api/reminders", handler.PostReminder)
	router.PUT("/api/reminders/:id", handler.PutReminder)
	router.DELETE("/api/reminders/:id", handler.DeleteReminder)
	return router
}

func do(t *testing.T, router *gin.Engine, method, path string, body any) *httptest.ResponseRecorder {
	t.Helper()
	var buf bytes.Buffer
	if body != nil {
		if s, ok := body.(string); ok {
			buf.WriteString(s)
		} else if err := json.NewEncoder(&buf).Encode(body); err != nil {
			t.Fatal(err)
		}
	}
	req := httptest.NewRequest(method, path, &buf)
	req.Header.Set("Content-Type", "application/json")
	rec := httptest.NewRecorder()
	router.ServeHTTP(rec, req)
	return rec
}

func decode[T any](t *testing.T, rec *httptest.ResponseRecorder) T {
	t.Helper()
	var out T
	if err := json.Unmarshal(rec.Body.Bytes(), &out); err != nil {
		t.Fatalf("invalid JSON body %q: %v", rec.Body.String(), err)
	}
	return out
}

func TestGetReminders(t *testing.T) {
	t.Run("returns the list", func(t *testing.T) {
		router := newRouter(&fakeRepository{getAll: func() ([]models.Reminder, error) {
			return []models.Reminder{sample}, nil
		}})
		rec := do(t, router, http.MethodGet, "/api/reminders", nil)
		if rec.Code != http.StatusOK {
			t.Fatalf("status = %d, want 200", rec.Code)
		}
		got := decode[[]models.Reminder](t, rec)
		if len(got) != 1 || got[0].Id != sample.Id || got[0].Title != sample.Title {
			t.Fatalf("body = %+v", got)
		}
	})

	t.Run("nil list becomes empty array", func(t *testing.T) {
		router := newRouter(&fakeRepository{getAll: func() ([]models.Reminder, error) { return nil, nil }})
		rec := do(t, router, http.MethodGet, "/api/reminders", nil)
		if rec.Code != http.StatusOK || bytes.TrimSpace(rec.Body.Bytes())[0] != '[' {
			t.Fatalf("status = %d body = %q, want 200 and a JSON array", rec.Code, rec.Body.String())
		}
	})

	t.Run("repository error is 500", func(t *testing.T) {
		router := newRouter(&fakeRepository{getAll: func() ([]models.Reminder, error) { return nil, errors.New("boom") }})
		rec := do(t, router, http.MethodGet, "/api/reminders", nil)
		if rec.Code != http.StatusInternalServerError {
			t.Fatalf("status = %d, want 500", rec.Code)
		}
	})
}

func TestGetCount(t *testing.T) {
	t.Run("returns the count", func(t *testing.T) {
		router := newRouter(&fakeRepository{count: func() (int, error) { return 7, nil }})
		rec := do(t, router, http.MethodGet, "/api/reminders/count", nil)
		if rec.Code != http.StatusOK || decode[int](t, rec) != 7 {
			t.Fatalf("status = %d body = %q", rec.Code, rec.Body.String())
		}
	})

	t.Run("repository error is 500", func(t *testing.T) {
		router := newRouter(&fakeRepository{count: func() (int, error) { return 0, errors.New("boom") }})
		rec := do(t, router, http.MethodGet, "/api/reminders/count", nil)
		if rec.Code != http.StatusInternalServerError {
			t.Fatalf("status = %d, want 500", rec.Code)
		}
	})
}

func TestGetReminder(t *testing.T) {
	t.Run("returns the reminder", func(t *testing.T) {
		var asked string
		router := newRouter(&fakeRepository{getByID: func(id string) (models.Reminder, error) {
			asked = id
			return sample, nil
		}})
		rec := do(t, router, http.MethodGet, "/api/reminders/"+sample.Id, nil)
		if rec.Code != http.StatusOK || asked != sample.Id {
			t.Fatalf("status = %d asked = %q", rec.Code, asked)
		}
		if got := decode[models.Reminder](t, rec); got.Title != sample.Title {
			t.Fatalf("body = %+v", got)
		}
	})

	t.Run("unknown id is 404", func(t *testing.T) {
		router := newRouter(&fakeRepository{getByID: func(string) (models.Reminder, error) {
			return models.Reminder{}, ErrorReminderNotFound
		}})
		rec := do(t, router, http.MethodGet, "/api/reminders/missing", nil)
		if rec.Code != http.StatusNotFound {
			t.Fatalf("status = %d, want 404", rec.Code)
		}
	})

	t.Run("repository error is 500", func(t *testing.T) {
		router := newRouter(&fakeRepository{getByID: func(string) (models.Reminder, error) {
			return models.Reminder{}, errors.New("boom")
		}})
		rec := do(t, router, http.MethodGet, "/api/reminders/x", nil)
		if rec.Code != http.StatusInternalServerError {
			t.Fatalf("status = %d, want 500", rec.Code)
		}
	})
}

func TestPostReminder(t *testing.T) {
	t.Run("creates and returns 201", func(t *testing.T) {
		var received models.Reminder
		router := newRouter(&fakeRepository{create: func(r models.Reminder) (models.Reminder, error) {
			received = r
			r.Id = sample.Id
			return r, nil
		}})
		rec := do(t, router, http.MethodPost, "/api/reminders", map[string]any{
			"title": "Pay rent", "description": "Transfer", "limitDate": "2026-09-01T00:00:00Z", "isDone": false,
		})
		if rec.Code != http.StatusCreated {
			t.Fatalf("status = %d body = %q, want 201", rec.Code, rec.Body.String())
		}
		if received.Title != "Pay rent" || !received.LimitDate.Equal(sample.LimitDate) {
			t.Fatalf("repository received %+v", received)
		}
		if got := decode[models.Reminder](t, rec); got.Id != sample.Id {
			t.Fatalf("body = %+v", got)
		}
	})

	t.Run("invalid JSON is 400", func(t *testing.T) {
		router := newRouter(&fakeRepository{})
		rec := do(t, router, http.MethodPost, "/api/reminders", "{not json")
		if rec.Code != http.StatusBadRequest {
			t.Fatalf("status = %d, want 400", rec.Code)
		}
	})

	t.Run("repository error is 500", func(t *testing.T) {
		router := newRouter(&fakeRepository{create: func(models.Reminder) (models.Reminder, error) {
			return models.Reminder{}, errors.New("boom")
		}})
		rec := do(t, router, http.MethodPost, "/api/reminders", sample)
		if rec.Code != http.StatusInternalServerError {
			t.Fatalf("status = %d, want 500", rec.Code)
		}
	})
}

func TestPutReminder(t *testing.T) {
	t.Run("updates and returns 200", func(t *testing.T) {
		var askedID string
		router := newRouter(&fakeRepository{update: func(id string, r models.Reminder) (models.Reminder, error) {
			askedID = id
			r.Id = id
			return r, nil
		}})
		body := sample
		body.IsDone = true
		rec := do(t, router, http.MethodPut, "/api/reminders/"+sample.Id, body)
		if rec.Code != http.StatusOK || askedID != sample.Id {
			t.Fatalf("status = %d asked = %q", rec.Code, askedID)
		}
		if got := decode[models.Reminder](t, rec); !got.IsDone || got.Id != sample.Id {
			t.Fatalf("body = %+v", got)
		}
	})

	t.Run("invalid JSON is 400", func(t *testing.T) {
		router := newRouter(&fakeRepository{})
		rec := do(t, router, http.MethodPut, "/api/reminders/"+sample.Id, "nope")
		if rec.Code != http.StatusBadRequest {
			t.Fatalf("status = %d, want 400", rec.Code)
		}
	})

	t.Run("unknown id is 404", func(t *testing.T) {
		router := newRouter(&fakeRepository{update: func(string, models.Reminder) (models.Reminder, error) {
			return models.Reminder{}, ErrorReminderNotFound
		}})
		rec := do(t, router, http.MethodPut, "/api/reminders/missing", sample)
		if rec.Code != http.StatusNotFound {
			t.Fatalf("status = %d, want 404", rec.Code)
		}
	})

	t.Run("repository error is 500", func(t *testing.T) {
		router := newRouter(&fakeRepository{update: func(string, models.Reminder) (models.Reminder, error) {
			return models.Reminder{}, errors.New("boom")
		}})
		rec := do(t, router, http.MethodPut, "/api/reminders/"+sample.Id, sample)
		if rec.Code != http.StatusInternalServerError {
			t.Fatalf("status = %d, want 500", rec.Code)
		}
	})
}

func TestDeleteReminder(t *testing.T) {
	t.Run("deletes and echoes the id", func(t *testing.T) {
		var askedID string
		router := newRouter(&fakeRepository{delete: func(id string) error { askedID = id; return nil }})
		rec := do(t, router, http.MethodDelete, "/api/reminders/"+sample.Id, nil)
		if rec.Code != http.StatusOK || askedID != sample.Id || decode[string](t, rec) != sample.Id {
			t.Fatalf("status = %d asked = %q body = %q", rec.Code, askedID, rec.Body.String())
		}
	})

	t.Run("repository error is 500", func(t *testing.T) {
		router := newRouter(&fakeRepository{delete: func(string) error { return errors.New("boom") }})
		rec := do(t, router, http.MethodDelete, "/api/reminders/x", nil)
		if rec.Code != http.StatusInternalServerError {
			t.Fatalf("status = %d, want 500", rec.Code)
		}
	})
}
