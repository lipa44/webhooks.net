﻿namespace Octokit.Webhooks.Events;

using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Octokit.Webhooks.Converter;
using Octokit.Webhooks.Models;
using Octokit.Webhooks.Models.PullRequestReviewEvent;

[PublicAPI]
[WebhookEventType(WebhookEventType.PullRequestReview)]
[JsonConverter(typeof(WebhookConverter<PullRequestReviewEvent>))]
public abstract record PullRequestReviewEvent : WebhookEvent
{
    [JsonPropertyName("review")]
    public Review Review { get; init; } = null!;

    [JsonPropertyName("pull_request")]
    public SimplePullRequest PullRequest { get; init; } = null!;
}
