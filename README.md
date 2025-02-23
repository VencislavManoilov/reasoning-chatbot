# Chatbot with Reasoning

Welcome to the Chatbot with Reasoning project! This repository contains a feature-rich chatbot application that utilizes OpenAI's API to provide advanced language model processing. The chatbot offers multiple operation modes, including reasoning capabilities for enhanced conversational interactions. The reasoning isn't just using the o1 model but creating multible steps and solutions for solving the problem which creates a chain of thought.

## Technologies Used

- **Backend**: ASP.NET
- **Frontend**: Angular
- **Database**: PostgreSQL
- **APIs**: OpenAI API

## Features

- **Authentication**: Secure user authentication to protect conversations.
- **Chat History**: Maintain a history of user conversations.
- **Model Selection**:
  - **Normal**: Utilizes `gpt-4o-mini`.
  - **Normal++**: Utilizes `gpt-4o`.
  - **Reason**: Utilizes `gpt-4o-mini` with reasoning capabilities.
  - **Reason++**: Utilizes `gpt-4o` with reasoning capabilities.

## Running Locally

To run this project locally, you need to follow these steps:

### Frontend

1. Navigate to the client directory:
   ```bash
   cd ./client
   ```

2. Install the necessary npm packages:
   ```bash
   npm install
   ```

3. Start the Angular development server:
   ```bash
   npm start
   ```

### Backend

1. Ensure you have an `.env` file present similar to [`.env.example`](./backend/.env.example).

2. Add your OpenAI API key and your PostgreSQL database connection string to the `.env` file.

3. Run the backend (Make sure you have `.NET` installed):
   ```bash
   dotnet run
   ```

### Database

Ensure your PostgreSQL database is running with the necessary tables created. As of now, tables are not automatically created but I am working on it.

## Running with Docker

You can also run this project using Docker. Please note that database tables are not created automatically yet, and work is ongoing to rectify this.

1. Create a `docker-compose.yml` file using the `docker-compose.example.yml` and paste your own OpenAI API key

2. Use the following command to build and start the Docker containers:
   ```bash
   docker-compose up --build
   ```

3. The database container isn't creating the tables automatically so it won't work but I am trying to fix this.

Feel free to contribute to this project or raise issues if you encounter any problems!
