﻿namespace Money.API.Requests;

public class CreateParticipantRequest
{
    public required string Name { get; set; }
    public required string CPF { get; set; }
}