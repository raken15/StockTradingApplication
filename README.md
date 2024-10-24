# StockTradingApplication

## Overview

**StockTradingApplication** is a fully-featured stock trading platform built using C# and WPF, leveraging the **MVVM (Model-View-ViewModel)** pattern to ensure separation of concerns and maintainability. It allows users to buy and sell stocks, manage their financial portfolio, and track real-time stock price updates. 

The solution also includes a comprehensive unit testing project to ensure the robustness and reliability of the application’s core functionalities. 

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
  - [MVVM Pattern](#mvvm-pattern)
  - [Key Components](#key-components)
  - [Design and Code Improvements](#design-and-code-improvements)
- [Flow and Structure](#flow-and-structure)
- [Installation](#installation)
- [Usage](#usage)
- [Logging](#logging)
- [Tests](#tests)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Real-time stock price updates** with periodic randomization for simulation.
- **Buy and sell stocks** with price and quantity conditions.
- **Financial portfolio management**: Track assets and evaluate portfolio performance.
- Implements the **MVVM pattern** for maintainability and testability.
- **Robust unit tests** to ensure code reliability.
- **Custom commands** using `RelayCommand` for efficient interaction between ViewModels and the UI.
- **Logging** with `SimpleLogger` for tracking application events and debugging.

## Architecture

The application follows the **MVVM architecture**. This design pattern enforces a clear separation of concerns, promoting a structured and maintainable codebase. 

### MVVM Pattern

- **Model**: Encapsulates business logic and data representation. Examples include `StockModel` and `FinancialPortfolioModel`.
- **View**: Defines the UI using XAML. For instance, `MainWindow.xaml` is responsible for displaying the main user interface.
- **ViewModel**: Acts as a bridge between the Model and View. ViewModels like `MainViewModel` and `StockViewModel` expose data and commands to the View and handle user interaction logic.

### Key Components

- **MainViewModel**: The primary ViewModel managing the stock data, user operations, and overall application state.
- **StockModelRepository**: Manages stock-related data, encapsulating data access logic and business operations.
- **FinancialPortfolioViewModel**: Manages the user's portfolio, handling stock buy/sell operations and updating the UI accordingly.
- **RelayCommand**: Implements command logic, binding actions (such as button clicks) to methods in the ViewModel.
- **SimpleLogger**: Provides logging functionality to record important events, errors, and messages during the application's runtime.

### Design and Code Improvements

- **MVVM structure**: Promotes loose coupling between UI (View) and business logic (ViewModel), making the code easier to test and maintain.
- **Command pattern**: Utilizes `RelayCommand`, providing flexibility in executing UI commands and supporting validation (e.g., `CanExecute`).
- **Price update simulation**: The stock prices are updated in real-time with a timer-based event handler for simulating stock market fluctuations.
- **Logging with `SimpleLogger`**: Centralized logging captures important events (e.g., stock price updates, successful trades) and errors for debugging and monitoring.
- **Dependency Injection (DI)**: Ensures that ViewModels and services are easily testable and interchangeable, facilitating future modifications.

## Flow and Structure

```text
StockTradingApplication/
├── Assets/
│   └── InitialSettings.txt            # Initial configuration settings for stock prices, limits, etc.
├── Helpers/
│   ├── RelayCommand.cs                # Command class for handling user actions.
│   ├── SimpleLogger.cs                # Custom logger for tracking events and errors.
├── Models/
│   ├── FinancialPortfolioModel.cs     # Represents user’s portfolio data (e.g., stocks owned, value).
│   └── StockModel.cs                  # Represents stock details (symbol, price, quantity).
├── Repositories/
│   ├── IRepository.cs                 # Generic repository interface for data access.
│   └── StockModelRepository.cs        # Implements stock data retrieval and storage.
├── ViewModels/
│   ├── FinancialPortfolioViewModel.cs # Handles portfolio-related logic and updates.
│   ├── MainViewModel.cs               # Core ViewModel, binding UI to stock and portfolio data.
│   └── StockViewModel.cs              # Handles logic for individual stock items.
├── Views/
│   ├── MainWindow.xaml                # UI definition for the main window.
│   └── App.xaml.cs                    # Application entry point.
└── Tests/
    └── MainWindowViewModelTests.cs    # Unit tests for validating MainViewModel behavior.
```
# Application Flow

- **Initialization**: The app reads `InitialSettings.txt` for stock limits and portfolio values.
- **Real-time Updates**: Stock prices are periodically updated by the `MainViewModel`, which triggers UI updates.
- **User Interaction**: The user can buy or sell stocks, with conditions applied to the price and quantity.
- **Portfolio Management**: The portfolio is updated based on successful trades, and the `FinancialPortfolioViewModel` handles this data binding.
- **Victory/Loss Conditions**: The game logic is based on financial thresholds defined in the settings.

# Installation

## Prerequisites

- Visual Studio
- .NET 8.0 SDK

## Steps

1. Clone the repository:
    ```bash
    git clone https://github.com/YourUsername/StockTradingApplication.git
    ```

2. Navigate to the project directory:
    ```bash
    cd StockTradingApplication
    ```

3. Open the solution in Visual Studio.

4. Build the solution to restore all dependencies.

# Usage

1. Build the solution.
2. Run the application using Visual Studio or from the executable in the bin folder.
3. Use the UI to manage stock trading activities. Prices update automatically, and you can monitor your financial portfolio's performance.

# Commands and Controls

- **Buy Stocks**: Purchase stocks from the market based on price conditions.
- **Sell Stocks**: Sell owned stocks, updating the portfolio.
- **Real-time Updates**: Observe live changes in stock prices.
- **Win/Loss Conditions**: The game shows an overlay when specific thresholds are reached, either winning or losing the game.

# Logging

The application uses a custom logging mechanism (`SimpleLogger.cs`) to track important events:

- Logs stock price updates, buy/sell operations, and exceptions.
- Stores logs in a file for later debugging or real-time monitoring.

To enable logging, configure the log path and desired log levels in the logger settings.

# Tests

The `Tests` folder contains unit tests to ensure the functionality of the ViewModels:

- **MainViewModelTests**: Verifies the core logic for managing stock data and user interaction.
- **FinancialPortfolioViewModelTests**: Ensures the portfolio updates correctly after trades.

## To run tests:

1. Navigate to the Tests directory:
    ```bash
    cd Tests
    ```

2. Run the tests using the .NET CLI:
    ```bash
    dotnet test
    ```

3. Review the results to ensure all tests pass.

# Contributing

Contributions are welcome! To contribute:

1. Fork the repository.

2. Create a new branch with your feature/fix:
    ```bash
    git checkout -b feature-name
    ```

3. Commit your changes and open a pull request.

Ensure all changes are tested before submitting a PR.

# License

This project is licensed under the MIT License. See the LICENSE file for details.
