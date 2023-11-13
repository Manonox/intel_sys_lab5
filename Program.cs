using Production;

var model = new Model();


foreach (var line in File.ReadAllLines("facts.txt")) {
    var split = line.Split(": ");
    model.AddFact(split[0], split[1]);
}

foreach (var line in File.ReadAllLines("rules.txt")) {
    var split = line.Split("=");
    model.AddRule(split[0].Split(","), split[1].Split(","));
}

var (current, target) = (new[] { "F104", "F106", "F201", "F206", "F203" }, new[] { "F404" });


/*
model.AddRule(new[] { "a", "b" }, new[] { "trash1" });
model.AddRule(new[] { "a", "c" }, new[] { "trash2" });
model.AddRule(new[] { "trash1", "trash2" }, new[] { "trash3" });

model.AddRule(new[] { "a", "b" }, new[] { "c" } );

model.AddRule(new[] { "b", "c" }, new[] { "d", "trash3" });

model.AddRule(new[] { "c", "a" }, new[] { "g", "trash4" });
model.AddRule(new[] { "b", "d" }, new[] { "f", "funny" });

var (current, target) = (new[] { "a", "b" }, new[] { "f", "g" });
*/


Console.OutputEncoding = System.Text.Encoding.UTF8;
var printResult = (Solver.Result result) => {
    Console.WriteLine("> Success: " + result.Success);
    if (result.Success) {
        Console.WriteLine("> Productions:");
        foreach (var rule in result.Rules) {
            Console.WriteLine("\t" + rule.ToStringWithDescriptions(model));
        }
    }
};


Solver solver;
Solver.Result result;



// ====================== //

Console.WriteLine("\nForwardSolver");
solver = new ForwardSolver(model);
result = solver.Solve(current, target);
printResult(result);

Console.WriteLine("\nBackwardSolver");
solver = new BackwardSolver(model);
result = solver.Solve(current, target);
printResult(result);


// Console.ReadLine();
