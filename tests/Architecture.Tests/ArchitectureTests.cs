using NetArchTest.Rules;

namespace Architecture.Tests;

public class ArchitectureTests
{
    private const string DomainNamespace = "Domain";
    private const string ApplicationNamespace = "Application";
    private const string InfrastructureNamespace = "Infrastructure";
    private const string ApiNamespace = "Api";

    [Fact]
    public void Domain_Should_Not_Depend_On_Other_Projects()
    {
        // Act
        var result = Types
            .InAssembly(typeof(Domain.Entities.Story).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, ApiNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        // Act
        var result = Types
            .InAssembly(typeof(Application.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, ApiNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        // Act
        var result = Types
            .InAssembly(typeof(Infrastructure.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Api_Should_Not_Be_Depended_On_By_Other_Projects()
    {
        // Act
        var domainResult = Types
            .InAssembly(typeof(Domain.Entities.Story).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        var applicationResult = Types
            .InAssembly(typeof(Application.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        var infrastructureResult = Types
            .InAssembly(typeof(Infrastructure.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        // Assert
        Assert.True(domainResult.IsSuccessful);
        Assert.True(applicationResult.IsSuccessful);
        Assert.True(infrastructureResult.IsSuccessful);
    }


    [Fact]
    public void Api_Should_Not_Depend_On_Domain()
    {
        // Act
        var result = Types
            .InAssembly(typeof(Api.Controllers.StoriesController).Assembly)
            .ShouldNot()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api_Or_Reference_Api_Types()
    {
        // Act
        var result = Types
            .InAssembly(typeof(Infrastructure.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }
}
