// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 7th 2026
//  Description: Logging file
//               Implementing Serilog
// --------------------------------------------



using Serilog;

namespace Backend.API.src.Core.Logging;

/// <summary>
/// ✨ The Lighthouse: Our central logging station 
/// Logger class for the back-end
/// </summary>
public static class AppLogger
{
    // 1. Debug level
    public static void DebugState(string component, string message, object? data = null)
    { 
        // Using "Structured Logging" ({Data} syntax)
        Log.Debug("[DEBUG][{Component}] {Message} | Data: {@Data}", component, message, data);
    }

    // 2. Information level options
    //  User Action Information
    public static void UserAction(string userId, string action)
    {
        Log.Information("[USER][{UserId}] performed: {Action}", userId, action);
    }

    //  Connection Logic
    public static void ConnectionEvent(string connectionId, string status, string? userId = null )
    {
        Log.Information("[CONN][{Status}] ID: {ConnectionId} | User: {userId}", status, connectionId, userId ?? "Anonymous");
    }

    //  Data Logic
    public static void DataStore(string operation, string collection, bool success)
    {
        Log.Information("[DATA][{Operation}] Collection: {Collection} | Success: {Success}", operation, collection, success);
    }

    //  Message Logic
    public static void MessageBus(string exchange, string routingKey, string action)
    {
        Log.Information("[BUS][{Action}] Exchange: {Exchange} | Key: {routingKey}", action, exchange, routingKey);
    }

    // 3. Warning Level
    public static void MPerformance(string operation, long elapsedMilliseconds)
    {
       if (elapsedMilliseconds > 500)
        Log.Warning("[SLOW] {Operation} took {Elapsed}ms", operation, elapsedMilliseconds);
    }

    // 4. Error level 
    public static void ShieldFailure(string area, Exception ex)
    {
        Log.Error(ex, "[SHIELD FAILURE] Error in {Area}: {ErrorMessage}", area, ex.Message);
    }
}
