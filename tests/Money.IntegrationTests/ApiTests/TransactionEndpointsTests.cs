﻿using Money.API.Endpoints.Shared;
using Money.Domain.Entities;
using Money.TestsShared.Builders.Domain.Requests;
using System.Net;
using System.Net.Http.Json;

namespace Money.IntegrationTests.ApiTests;
public class TransactionEndpointsTests : BaseIntegrationTests
{
    private HttpClient _httpClient;
    private const string _baseUri = "/api/v1/transactions";

    [SetUp]
    public void SetUp()
    {
        _httpClient = TestApi.CreateClient();
    }

    [Test]
    public async Task CreditShouldReturnSuccess()
    {
        var createCreditTransactionRequest = new CreateCreditTransactionRequestBuilder().Generate();

        var response = await _httpClient.PostAsync($"{_baseUri}/credit", createCreditTransactionRequest.ToJsonContent());

        response.Should().HaveStatusCode(HttpStatusCode.OK);
    }

    [Test]
    public async Task CreateShouldReturnBadRequestWhenRequestIsInvalid()
    {
        var createCreditTransactionRequest = new CreateCreditTransactionRequestBuilder()
            .WithAmount(0)
            .Generate();

        var response = await _httpClient.PostAsync($"{_baseUri}/credit", createCreditTransactionRequest.ToJsonContent());

        var problemDetails = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

        var customProblemDetailsExpected = CustomProblemDetails.CreateValidationProblemDetails(
            $"{_baseUri}/credit",
            new List<CustomProblemDetailsError>()
            {
                new("Amount", "'Amount' must be greater than '0'.")
            });

        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        problemDetails.Should().BeEquivalentTo(customProblemDetailsExpected, ctx => ctx.Excluding(p => p.Type));
    }
}