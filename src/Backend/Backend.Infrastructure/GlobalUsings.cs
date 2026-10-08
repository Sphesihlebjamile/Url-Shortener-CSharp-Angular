global using System.Data;
global using System.Diagnostics.CodeAnalysis;
global using Backend.Application.Capabilities;
global using Backend.Application.Exceptions;
global using Backend.Application.Options;
global using Backend.Application.Orchestrators;
global using Backend.Application.Persistence;
global using Backend.Application.Plans;
global using Backend.Contracts.ApiRequestTypes.DataEndpoint;
global using Backend.Contracts.ApiResponseTypes.DataEndpoint;
global using Backend.Domain.Constants;
global using Dapper;
global using Microsoft.Data.SqlClient;
global using Microsoft.Extensions.Options;

namespace Backend.Infrastructure;
