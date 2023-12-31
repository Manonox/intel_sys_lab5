namespace Production {

    class BackwardSolver : Solver {
        public ForwardSolver forwardSolver;

        public BackwardSolver(Model model) : base(model) {
            var invertedRules = InvertRules(model.Rules);
            var invertedModel = new Model(model.Facts, invertedRules);
            forwardSolver = new ForwardSolver(invertedModel);
        }

        static Model.Rule InvertRule(Model.Rule rule) {
            var invertedRule = new Model.Rule(rule);
            (invertedRule.From, invertedRule.To) = (invertedRule.To, invertedRule.From);
            return invertedRule;
        }

        static List<Model.Rule> InvertRules(List<Model.Rule> rules) {
            var invertedRules = new List<Model.Rule>();
            foreach (var rule in rules)
                invertedRules.Add(InvertRule(rule));
            return invertedRules;
        }

        public override Result Solve(IEnumerable<string> _current, IEnumerable<string> _target) {
            var current = _current.ToHashSet(); var target = _target.ToHashSet();
            var rules = Model.Rules;

            var result = new Result() { Success = false };

            // Build a tree of required productions
            var stack = new Stack<string>(target);
            var factProducingRules = new Dictionary<string, Model.Rule>();
            var appliedRules = new HashSet<Model.Rule>();
            var backPropagatedRules = new HashSet<Model.Rule>();
            while (stack.Any() && !current.IsSupersetOf(target)) {
                var next = stack.Peek();
                if (current.Contains(next)) {
                    stack.Pop();
                    continue;
                }
                
                var usefulRules = rules.Where((rule) => rule.To.Contains(next));
                if (!usefulRules.Any()) {
                    stack.Pop();
                    continue;
                }

                var applicableRules = usefulRules.Where((rule) => rule.IsApplicable(current));
                if (applicableRules.Any()) {
                    var rule = applicableRules.First();
                    
                    // Apply rule
                    current.UnionWith(rule.To);
                    foreach (var fact in rule.To) {
                        factProducingRules[fact] = rule;
                    }

                    appliedRules.Add(rule);
                    continue;
                }

                var backPropagationRules = usefulRules.Where((rule) => !backPropagatedRules.Contains(rule));
                if (!backPropagationRules.Any()) {
                    stack.Pop();
                    continue;
                }

                foreach (var rule in backPropagationRules) {
                    backPropagatedRules.Add(rule);
                    
                    var missingFacts = rule.From.Where((fact) => !current.Contains(fact));
                    foreach (var fact in missingFacts)
                        stack.Push(fact);
                }
            }

            if (!target.IsSubsetOf(current))
                return result;
            result.Success = true;

            // Traverse the trees, starting at root (target) nodes
            stack = new Stack<string>(target);
            while (stack.Any()) {
                var fact = stack.Pop();
                if (!factProducingRules.ContainsKey(fact))
                    continue;
                var rule = factProducingRules[fact];

                result.Rules.Add(rule);
                foreach (var fromFact in rule.From)
                    stack.Push(fromFact);
            }

            // Remove duplicates from intersecting trees
            var seenRules = new HashSet<Model.Rule>();
            for (var i = result.Rules.Count - 1; i >= 0; i--) {
                var rule = result.Rules[i];
                if (seenRules.Contains(rule)) {
                    result.Rules.RemoveAt(i);
                    continue;
                }
                seenRules.Add(rule);
            }

            result.Rules.Reverse();
            // Because we begin from the roots (targets)
            
            return result;
        }
    }
}
