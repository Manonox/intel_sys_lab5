using Production;

var model = new Model();


model.AddRule(new[] { "a", "b" }, new[] { "trash1" });
model.AddRule(new[] { "a", "c" }, new[] { "trash2" });
model.AddRule(new[] { "trash1", "trash2" }, new[] { "trash3" });

model.AddRule(new[] { "a", "b" }, new[] { "c" } );

model.AddRule(new[] { "b", "c" }, new[] { "d" });

model.AddRule(new[] { "c", "a" }, new[] { "g" });
model.AddRule(new[] { "b", "d" }, new[] { "f" });


var printResult = (Solver.Result result) => {
    Console.WriteLine("> Success: " + result.Success);
    if (result.Success) {
        Console.WriteLine("> Productions:");
        foreach (var rule in result.Rules) {
            Console.WriteLine("\t" + rule);
        }
    }
};


Solver solver;
Solver.Result result;



// ====================== //

Console.WriteLine("\nForwardSolver");
solver = new ForwardSolver(model);
result = solver.Solve(new[] { "a", "b" }, new[] { "f", "g" });
printResult(result);

Console.WriteLine("\nBackwardSolver");
solver = new BackwardSolver(model);
result = solver.Solve(new[] { "a", "b" }, new[] { "f", "g" });
printResult(result);


// Console.ReadLine();
