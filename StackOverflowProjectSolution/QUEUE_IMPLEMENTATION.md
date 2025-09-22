# Azure Storage Queue Implementation

## Overview

This implementation adds Azure Storage Queue for asynchronous notification processing between services in the CloudAzureService application.

## Architecture

```
StackOverflowService ---> Azure Storage Queue ---> NotificationService
HealthMonitoringService -----------------------> NotificationService
```

## Queue Message Types

### 1. BestAnswerSelected
Sent when a question owner marks an answer as the best answer.

**Triggers:** StackOverflowService -> QuestionsController.MarkBestAnswer()

**Recipients:** 
- Answer author (person who wrote the selected answer)
- Question author (person who asked the question)

**Data:**
- QuestionTitle
- QuestionId
- AnswerId
- AnswerAuthorName
- QuestionAuthorName

### 2. ServiceHealthAlert
Sent when health monitoring detects a service is unavailable.

**Triggers:** HealthMonitoringService -> WorkerRole (exception handling)

**Recipients:** All configured alert email addresses

**Data:**
- ServiceName (StackOverflowService or NotificationService)
- Status (not_ok)
- ErrorMessage
- Timestamp

## Configuration

The queue uses the same `DataConnectionString` as other Azure Storage services in the application.

Queue name: `notifications` (configurable via AzureQueueService constructor)

## Service Implementations

### StackOverflowService
- Modified `QuestionsController.MarkBestAnswer()` to send queue notifications
- Added async support for queue operations
- Graceful error handling - best answer marking succeeds even if queue fails

### HealthMonitoringService
- Modified `WorkerRole.RunAsync()` exception handling
- Sends service health alerts via queue instead of direct email
- Fallback to direct email if queue fails

### NotificationService
- Added queue consumer in `WorkerRole.RunAsync()`
- Processes messages every 10 seconds when queue is empty
- Creates specialized email templates for different message types
- Handles both notification types with appropriate email content

## Benefits

1. **Decoupled Communication**: Services don't need direct connections
2. **Reliability**: Messages are persisted in Azure Storage Queue
3. **Scalability**: Asynchronous processing improves response times
4. **Fault Tolerance**: Fallback mechanisms for error scenarios
5. **Monitoring**: Comprehensive logging for message flow tracking

## Error Handling

- Queue initialization failures are logged and throw exceptions
- Message sending failures are logged and propagated
- Queue consumer continues processing even if individual messages fail
- Fallback to direct email sending for health monitoring alerts

## Usage

The queue system is automatically initialized when services start. No additional configuration is required beyond ensuring the `DataConnectionString` is properly set for Azure Storage access.