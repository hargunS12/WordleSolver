
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WordleSolver.Services;
using WordleSolver.Strategies;
using WordleSolver.Models;


var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        // Core game logic
        services.AddSingleton<WordleService>();

        // Student-supplied strategy
        services.AddSingleton<IWordleSolverStrategy, FantasticStudentSolver>();

        // Driver that runs many games
        services.AddSingleton<StudentGuesserService>();
    })
    .Build();

var runner = host.Services.GetRequiredService<StudentGuesserService>();
runner.Run(1000);  