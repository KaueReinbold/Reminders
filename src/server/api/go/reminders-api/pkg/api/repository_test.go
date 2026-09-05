package api

import (
	"database/sql"
	"errors"
	"reminders-go-api/pkg/models"
	"testing"
	"time"

	"github.com/DATA-DOG/go-sqlmock"
)

var columns = []string{"Id", "Title", "Description", "LimitDate", "IsDone"}

func newMockRepo(t *testing.T) (*PostgresReminderRepository, sqlmock.Sqlmock) {
	t.Helper()
	db, mock, err := sqlmock.New(sqlmock.QueryMatcherOption(sqlmock.QueryMatcherRegexp))
	if err != nil {
		t.Fatal(err)
	}
	t.Cleanup(func() {
		db.Close()
		if err := mock.ExpectationsWereMet(); err != nil {
			t.Errorf("unmet expectations: %v", err)
		}
	})
	return NewPostgresReminderRepository(db), mock
}

func TestRepositoryGetAll(t *testing.T) {
	t.Run("maps rows and excludes deleted via the query", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectQuery(`SELECT(?s:.*)FROM "Reminders"(?s:.*)"IsDeleted" != true`).
			WillReturnRows(sqlmock.NewRows(columns).
				AddRow(sample.Id, sample.Title, sample.Description, sample.LimitDate, sample.IsDone).
				AddRow("id-2", "Second", "", sample.LimitDate, true))

		got, err := repo.GetAll()
		if err != nil || len(got) != 2 || got[0].Title != "Pay rent" || !got[1].IsDone {
			t.Fatalf("got %+v err %v", got, err)
		}
	})

	t.Run("empty table yields empty slice, not nil", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectQuery(`SELECT`).WillReturnRows(sqlmock.NewRows(columns))

		got, err := repo.GetAll()
		if err != nil || got == nil || len(got) != 0 {
			t.Fatalf("got %#v err %v", got, err)
		}
	})

	t.Run("query error propagates", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectQuery(`SELECT`).WillReturnError(errors.New("down"))

		if _, err := repo.GetAll(); err == nil {
			t.Fatal("expected error")
		}
	})

	t.Run("scan error propagates", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectQuery(`SELECT`).WillReturnRows(sqlmock.NewRows(columns).
			AddRow(sample.Id, sample.Title, sample.Description, "not a time", sample.IsDone))

		if _, err := repo.GetAll(); err == nil {
			t.Fatal("expected scan error")
		}
	})
}

func TestRepositoryCount(t *testing.T) {
	t.Run("returns the count", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectQuery(`SELECT(?s:.*)COUNT\(\*\)`).WillReturnRows(sqlmock.NewRows([]string{"count"}).AddRow(3))

		got, err := repo.Count()
		if err != nil || got != 3 {
			t.Fatalf("got %d err %v", got, err)
		}
	})

	t.Run("query error propagates", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectQuery(`SELECT`).WillReturnError(errors.New("down"))

		if _, err := repo.Count(); err == nil {
			t.Fatal("expected error")
		}
	})
}

func TestRepositoryGetByID(t *testing.T) {
	t.Run("returns the reminder for the id", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectQuery(`WHERE(?s:.*)"Id" = \$1`).WithArgs(sample.Id).
			WillReturnRows(sqlmock.NewRows(columns).
				AddRow(sample.Id, sample.Title, sample.Description, sample.LimitDate, sample.IsDone))

		got, err := repo.GetByID(sample.Id)
		if err != nil || got.Id != sample.Id || got.Title != sample.Title {
			t.Fatalf("got %+v err %v", got, err)
		}
	})

	t.Run("no rows maps to ErrorReminderNotFound", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectQuery(`SELECT`).WithArgs("missing").WillReturnError(sql.ErrNoRows)

		_, err := repo.GetByID("missing")
		if !errors.Is(err, ErrorReminderNotFound) {
			t.Fatalf("err = %v, want ErrorReminderNotFound", err)
		}
	})

	t.Run("a real failure propagates instead of masquerading as not found", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		down := errors.New("down")
		mock.ExpectQuery(`SELECT`).WithArgs("x").WillReturnError(down)

		_, err := repo.GetByID("x")
		if !errors.Is(err, down) || errors.Is(err, ErrorReminderNotFound) {
			t.Fatalf("err = %v, want the database error", err)
		}
	})
}

func TestRepositoryCreate(t *testing.T) {
	t.Run("assigns a new id and inserts", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectExec(`INSERT INTO "Reminders"`).
			WithArgs(sqlmock.AnyArg(), sample.Title, sample.Description, sample.LimitDate, sample.IsDone, false).
			WillReturnResult(sqlmock.NewResult(0, 1))

		in := sample
		in.Id = ""
		got, err := repo.Create(in)
		if err != nil || got.Id == "" || got.Id == sample.Id || got.Title != sample.Title {
			t.Fatalf("got %+v err %v", got, err)
		}
	})

	t.Run("exec error propagates", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectExec(`INSERT`).WillReturnError(errors.New("down"))

		if _, err := repo.Create(sample); err == nil {
			t.Fatal("expected error")
		}
	})
}

func TestRepositoryUpdate(t *testing.T) {
	t.Run("updates by the route id, not the body id", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		later := time.Date(2026, 10, 1, 0, 0, 0, 0, time.UTC)
		mock.ExpectExec(`UPDATE "Reminders" SET`).
			WithArgs("New", "Desc", later, true, false, sample.Id).
			WillReturnResult(sqlmock.NewResult(0, 1))

		got, err := repo.Update(sample.Id, models.Reminder{Id: "other", Title: "New", Description: "Desc", LimitDate: later, IsDone: true})
		if err != nil || got.Id != sample.Id || !got.IsDone {
			t.Fatalf("got %+v err %v", got, err)
		}
	})

	t.Run("exec error propagates", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectExec(`UPDATE`).WillReturnError(errors.New("down"))

		if _, err := repo.Update(sample.Id, sample); err == nil {
			t.Fatal("expected error")
		}
	})
}

func TestRepositoryDelete(t *testing.T) {
	t.Run("soft deletes by id", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectExec(`UPDATE "Reminders" SET(?s:.*)"IsDeleted" = \$1`).
			WithArgs(true, sample.Id).
			WillReturnResult(sqlmock.NewResult(0, 1))

		if err := repo.Delete(sample.Id); err != nil {
			t.Fatal(err)
		}
	})

	t.Run("exec error propagates", func(t *testing.T) {
		repo, mock := newMockRepo(t)
		mock.ExpectExec(`UPDATE`).WillReturnError(errors.New("down"))

		if err := repo.Delete(sample.Id); err == nil {
			t.Fatal("expected error")
		}
	})
}
