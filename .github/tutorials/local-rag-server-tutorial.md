
# 🦾 Tutorial: Build Your Own Local RAG Server with Ollama + Mistral + Docker

This tutorial will teach you how to create your own 100% local Retrieval-Augmented Generation (RAG) solution from scratch. You’ll set up a project with Docker, Ollama, Mistral, ChromaDB, and LlamaIndex, and learn how to index your own code for local AI-powered search and Q&A.

---


## Prerequisites

- Docker and Docker Compose installed on your machine
- Python 3.10+ (for the RAG server)

---


## Step 1: Create the Project Structure

Create a new directory for your RAG server and set up the following structure:

```bash
mkdir -p rag_server/data/project
mkdir -p rag_server/server
cd rag_server
```

Your structure should look like:

```
rag_server/
├── data/
│   └── project/
├── server/
```

---


## Step 2: Add Your Project for Indexing

Copy the code you want to index into `rag_server/data/project/`. For example:

```
rag_server/data/project/
├── backend/
├── frontend/
├── README.md
```

This is the codebase the RAG server will index and answer questions about.

---


## Step 3: Write the Docker Compose File

Create a `docker-compose.yml` in `rag_server/` with the following content:

```yaml
version: '3.8'
services:
  ollama:
    image: ollama/ollama
    ports:
      - "11434:11434"
    volumes:
      - ollama_data:/root/.ollama

  chromadb:
    image: chromadb/chroma
    ports:
      - "8000:8000"
    volumes:
      - chroma_data:/chroma/.chroma/index

  rag-server:
    build: ./server
    volumes:
      - ./data/project:/app/project
    ports:
      - "5000:5000"
    depends_on:
      - ollama
      - chromadb

volumes:
  ollama_data:
  chroma_data:
```

This sets up Ollama, ChromaDB, and your custom RAG server.

---


## Step 4: Create the RAG Server Code

In `rag_server/server/`, create a minimal FastAPI app (or Flask) that uses LlamaIndex, InstructorEmbedding, and ChromaDB. Example `main.py`:

```python
# server/main.py
from fastapi import FastAPI, Request
from llama_index import VectorStoreIndex, SimpleDirectoryReader
from instructor_emb import INSTRUCTOR
import chromadb

app = FastAPI()

# Load and index project files
documents = SimpleDirectoryReader('project').load_data()
index = VectorStoreIndex.from_documents(documents)

@app.post("/query")
async def query(request: Request):
    data = await request.json()
    question = data["question"]
    response = index.query(question)
    return {"answer": response}
```

Create a `Dockerfile` in `server/`:

```dockerfile
# server/Dockerfile
FROM python:3.10-slim
WORKDIR /app
COPY . .
RUN pip install fastapi uvicorn llama-index instructor-embedding chromadb
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "5000"]
```

---


## Step 5: Build and Start Everything

From the `rag_server/` directory, run:

```bash
docker compose up --build
```

Ollama will download the `mistral` model on first run. The RAG server will index your code and persist the index.

## Step 6: Ask Questions About Your Code

Send POST requests to `http://localhost:5000/query` with a JSON body like:

```json
{
  "question": "Explain the authentication module"
}
```

You’ll get an answer based on your indexed code!

---

## Resources

- [LlamaIndex](https://llamaindex.ai/)
- [Ollama](https://ollama.com/)
- [Mistral](https://mistral.ai/)
- [ChromaDB](https://docs.trychroma.com/)

---

You have now built your own local RAG server from scratch. You can expand it, add authentication, or improve the UI as next steps!
