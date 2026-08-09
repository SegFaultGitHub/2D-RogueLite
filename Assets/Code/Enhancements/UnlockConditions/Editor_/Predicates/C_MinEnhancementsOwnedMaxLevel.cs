namespace Code.Enhancements.UnlockConditions.Editor_.Predicates {
    public class C_MinEnhancementsOwnedMaxLevel : C_Predicate {
        protected const string IN_PORT_COUNT = "Count";

        protected override string GetHeader() => "Owns X or more maxed-out enhancements";

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(IN_PORT_COUNT)
                .WithDisplayName("Count")
                .Build();
        }

        public override Runtime.C_Condition TranslateNode() {
            return new Runtime.Predicates.C_MinEnhancementsOwnedMaxLevel(
                GetInputPortValue<E_Mode>(this.GetInputPortByName(IN_PORT_MODE)),
                GetInputPortValue<int>(this.GetInputPortByName(IN_PORT_COUNT))
            );
        }
    }
}
