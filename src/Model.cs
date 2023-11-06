namespace Production {
    public class Model {
        public Dictionary<string, Fact> Facts { get; } = new Dictionary<string, Fact>();
        public List<Rule> Rules { get; } = new List<Rule>();

        public Model() { }
        public Model(Dictionary<string, Fact> facts, List<Rule> rules) {
            Facts = facts; Rules = rules;
        }

        public void AddFact(string name, string description) {
            Facts.Add(name, new Fact(name, description));
        }

        public void AddRule(IEnumerable<string> from, IEnumerable<string> to) {
            Rules.Add(new Rule(from, to));
        }



        public class Fact {
            public string Name { get; }
            public string? Description { get; }

            public Fact(string name, string? description) {
                Name = name; Description = description;
            }

            public override string ToString() {
                return Name;
            }
        }

        public class Rule {
            public HashSet<string> From { get; set; } = new HashSet<string>();
            public HashSet<string> To { get; set; } = new HashSet<string>();

            public Rule(IEnumerable<string> from, IEnumerable<string> to) {
                From = from.ToHashSet();
                To = to.ToHashSet();
            }
            
            public Rule(Rule other) {
                From = new HashSet<string>(other.From);
                To = new HashSet<string>(other.To);
            }
            
            public bool IsApplicable(HashSet<string> current) => From.IsSubsetOf(current);


            static string StringifyHashSet(HashSet<string> set) {
                string s = "[]";

                if (set.Count > 0) {
                    s = "[";
                    s += set.First().ToString();
                    bool skip = true;
                    foreach (var item in set) {
                        if (skip) { skip = false; continue; }
                        s += ", " + item.ToString();
                    }
                    s += "]";
                }

                return s;
            }

            public override string ToString() {
                return StringifyHashSet(From) + " => " + StringifyHashSet(To);
            }
        }
    }
}