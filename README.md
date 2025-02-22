# Chatbot with Reasoning

Welcome to the Chatbot with Reasoning project! This repository contains a feature-rich chatbot application that utilizes OpenAI's API to provide advanced language model processing. The chatbot offers multiple operation modes, including reasoning capabilities for enhanced conversational interactions.

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

3. Build and start the ASP.NET backend project. Adjust these steps according to your development environment.

### Database

Ensure your PostgreSQL database is running with the necessary tables created. As of now, tables are not automatically created using docker.

## Running with Docker

You can also run this project using Docker. Please note that database tables are not created automatically yet, and work is ongoing to rectify this.

1. Ensure Docker is installed and running on your machine.

2. Use the following command to build and start the Docker containers:
   ```bash
   docker-compose up --build
   ```

3. After running the above command, check the logs and ensure services have started correctly.

## Future Improvements

- Automate database table creation during Docker setup.

Feel free to contribute to this project or raise issues if you encounter any problems!
