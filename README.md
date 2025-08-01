# Personal Finance Management API

A comprehensive .NET Core application for managing personal financial transactions with advanced categorization and analytics capabilities.

## Features

### Core Functionality
- **Transaction Import**: Import bank transactions from CSV files with validation and error logging
- **Transaction Listing**: Paginated listing with advanced filtering (date range, transaction kind, etc.)
- **Category Management**: Import and manage spending categories with hierarchical structure
- **Transaction Categorization**: Categorize individual transactions manually or automatically
- **Transaction Splitting**: Split transactions into multiple categories with amount distribution
- **Spending Analytics**: Analytical views of spending by categories and subcategories

### Advanced Features
- **Auto-Categorization**: Rule-based automatic category assignment using configurable SQL predicates
- **CQRS Pattern**: Clean separation of commands and queries for better maintainability
- **Redis Caching**: Performance optimization with Redis integration
- **API Testing**: Comprehensive Postman test collection with Newman automation
- **Docker Support**: Fully containerized service with Docker Compose setup
- **Error Logging**: Detailed validation and business error logging system

## Tech Stack

- **.NET 8**: Latest .NET framework for high performance
- **Entity Framework Core**: ORM with PostgreSQL database
- **MediatR**: CQRS pattern implementation
- **AutoMapper**: Object-to-object mapping
- **Redis**: Caching and session storage
- **Docker**: Containerization and orchestration
- **Postman/Newman**: API testing and automation
- **CsvHelper**: CSV file processing

## API Endpoints

### Transactions
- `POST /transactions/import` - Import transactions from CSV
- `GET /transactions` - List transactions with filters and pagination
- `POST /transactions/{id}/categorize` - Categorize a single transaction
- `POST /transactions/{id}/split` - Split transaction into multiple categories
- `POST /transactions/auto-categorize` - Auto-categorize transactions using rules

### Categories
- `POST /categories/import` - Import categories from CSV
- `GET /categories` - List categories (with optional parent filtering)

### Analytics
- `GET /spending-analytics` - Get spending analysis by categories
