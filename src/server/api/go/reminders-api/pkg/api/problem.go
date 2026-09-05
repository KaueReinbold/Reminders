package api

import (
	"encoding/json"
	"net/http"

	"github.com/gin-gonic/gin"
)

// One error shape for the whole API: RFC 7807 problem details, matching the .NET
// and C++ implementations behind the same load balancer (ADR-0011).
const (
	ClientErrorType    = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
	ServerErrorType    = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
	ProblemContentType = "application/problem+json"
)

type ProblemDetails struct {
	Type   string              `json:"type"`
	Title  string              `json:"title"`
	Status int                 `json:"status"`
	Errors map[string][]string `json:"errors,omitempty"`
}

func writeProblem(c *gin.Context, problem ProblemDetails) {
	body, err := json.MarshalIndent(problem, "", "    ")

	if err != nil {
		c.Status(http.StatusInternalServerError)
		return
	}

	c.Data(problem.Status, ProblemContentType, body)
}

func problem(c *gin.Context, status int, title string) {
	problemType := ClientErrorType

	if status >= http.StatusInternalServerError {
		problemType = ServerErrorType
	}

	writeProblem(c, ProblemDetails{Type: problemType, Title: title, Status: status})
}

func validationProblem(c *gin.Context, errors map[string][]string) {
	writeProblem(c, ProblemDetails{
		Type:   ClientErrorType,
		Title:  "One or more validation errors occurred.",
		Status: http.StatusBadRequest,
		Errors: errors,
	})
}
