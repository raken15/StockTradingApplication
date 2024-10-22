# StockTradingApplication

## Overview
**StockTradingApplication** is a sophisticated stock trading platform built using C# and WPF, implementing the MVVM (Model-View-ViewModel) pattern. This repository also includes a comprehensive **Tests** project to ensure the reliability and accuracy of the application.

---

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Flow and Structure](#flow-and-structure)
- [Installation](#installation)
- [Usage](#usage)
- [Tests](#tests)
- [Contributing](#contributing)
- [License](#license)

---

## Features
- Real-time stock price updates
- Buy and sell stock functionality
- Detailed financial portfolio management
- MVVM pattern for maintainable and testable code
- Robust unit and integration tests

---

## Architecture

### MVVM Pattern
- **Model**: Contains business logic and data, such as `StockModelRepository`.
- **View**: The user interface (UI) elements.
- **ViewModel**: Acts as a bridge between the Model and the View, such as `MainViewModel`.

### Key Components
- **MainViewModel**: Central component managing data and business logic.
- **StockModelRepository**: Handles data access and storage.
- **FinancialPortfolioViewModel**: Manages user's financial portfolio.

### Dependency Injection
Utilizes dependency injection to promote loose coupling and testability.

### Flow and Structure

``` plaintext
+---------------------------------------------+
|                  MainViewModel              |
|---------------------------------------------|
| - Interacts with StockModelRepository       |
| - Manages FinancialPortfolioViewModel       |
| - Exposes properties and commands to View   |
+---------------------------------------------+
                |                   |
                |                   |
+---------------+                   +--------------------+
|                                                |
|                                                |
V                                               V
+----------------------------+             +-------------------------+
| StockModelRepository       |             | FinancialPortfolioVM    |
|----------------------------|             |-------------------------|
| - Abstracts data access    |             | - Updates portfolio     |
+----------------------------+             +-------------------------+
```
### Benefits
Separation of Concerns: Clear division between UI, business logic, and data.

Testability: Easy to write and maintain tests due to loose coupling.

Reusability: Reusable components and commands.

Performance: Efficient and responsive application.

### Installation
Clone the repository
``` sh
git clone https://github.com/YourUsername/StockTradingApplication.git
```
Navigate to the project directory:
``` sh
cd StockTradingApplication
```
Open the solution in Visual Studio and build the solution.
### Usage
Build the solution.
Run the application:
  Start the StockTradingApplication project from Visual Studio or from the exe file that is located in the bin folder of the StockTradingApplication project
Follow the prompts to manage your stock trading activities.

### Tests
Navigate to the Tests project directory.
Run the tests using your preferred test runner:
``` sh
dotnet test
```
Review the test results to ensure the application functions correctly.
### Contributing
Contributions are welcome! Please fork the repository and submit a pull request with your changes. Ensure your code adheres to the existing style and includes appropriate tests.
### License
This project is licensed under the MIT License - see the LICENSE file for details.
