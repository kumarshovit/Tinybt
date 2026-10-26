using System.Collections.Generic;

namespace TinyBtUrlApi.Core.Models;

/// <summary>
/// Encapsulates the combined outcome of the full URL validation pipeline:
/// format validation, redirect chain tracing (max 2 redirects), and parallel threat intelligence scans.
/// </summary>
public class UrlValidationPipelineResult
{
    public bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }
    public string? ThreatSource { get; init; }
    public string? ThreatType { get; init; }
    public string? FlaggedUrl { get; init; }
    public List<string> RedirectChain { get; init; } = new();
    public string? FinalUrl { get; init; }
    public bool IsAbuseAttempt => !string.IsNullOrEmpty(ThreatSource) 
                                  && ThreatSource != "redirect_limit" 
                                  && ThreatSource != "pending_or_inconclusive" 
                                  && ThreatSource != "api_error";

    public static UrlValidationPipelineResult Success(string finalUrl, List<string> chain) =>
        new()
        {
            IsValid = true,
            FinalUrl = finalUrl,
            RedirectChain = chain
        };

    public static UrlValidationPipelineResult RedirectLimitExceeded(string message, List<string> chain) =>
        new()
        {
            IsValid = false,
            ErrorMessage = message,
            ThreatSource = "redirect_limit",
            RedirectChain = chain
        };

    public static UrlValidationPipelineResult ThreatDetected(string source, string threatType, string flaggedUrl, string message, List<string> chain) =>
        new()
        {
            IsValid = false,
            ErrorMessage = message,
            ThreatSource = source,
            ThreatType = threatType,
            FlaggedUrl = flaggedUrl,
            RedirectChain = chain
        };

    public static UrlValidationPipelineResult InconclusiveOrPending(string message, List<string> chain) =>
        new()
        {
            IsValid = false,
            ErrorMessage = message,
            ThreatSource = "pending_or_inconclusive",
            RedirectChain = chain
        };

    public static UrlValidationPipelineResult ApiError(string message, List<string>? chain = null) =>
        new()
        {
            IsValid = false,
            ErrorMessage = message,
            ThreatSource = "api_error",
            RedirectChain = chain ?? new List<string>()
        };

    public static UrlValidationPipelineResult Failure(string message, List<string>? chain = null) =>
        new()
        {
            IsValid = false,
            ErrorMessage = message,
            ThreatSource = "validation_failure",
            RedirectChain = chain ?? new List<string>()
        };
}
