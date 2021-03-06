namespace Domain {
    public class Rule {
        public string Description;
        public int Value, MinVal, MaxVal;
        public RuleType RuleType;

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            if (typeof(Rule) != obj.GetType()) {
                return false;
            }

            var other = (Rule) obj;

            if (RuleType != other.RuleType) {
                return false;
            }

            return true;
        }

        public override int GetHashCode() {
            var hash = 3;

            hash = 53 * hash + RuleType.GetHashCode();

            return hash;
        }

        public Rule(Rule oldRule) {
            Description = oldRule.Description;
            Value = oldRule.Value;
            MinVal = oldRule.MinVal;
            MaxVal = oldRule.MaxVal;
            RuleType = oldRule.RuleType;
        }

        public Rule() { }
    }
}