var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.HFFitnessApp>("hffitnessapp");

builder.Build().Run();
