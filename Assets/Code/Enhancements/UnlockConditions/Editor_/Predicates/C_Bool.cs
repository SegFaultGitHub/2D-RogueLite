namespace Code.Enhancements.UnlockConditions.Editor_.Predicates {
    public class C_Bool : C_Predicate {
        private const string IN_PORT_BOOL = "Bool";

        protected override string GetHeader() {
            return "True / False";
        }

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            base.OnDefinePorts(context);

            context.AddInputPort<bool>(IN_PORT_BOOL)
                .WithDisplayName("Bool")
                .Build();
        }

        public override Runtime.C_Condition TranslateNode() {
            return new Runtime.Predicates.C_Bool(
                GetInputPortValue<E_Mode>(this.GetInputPortByName(IN_PORT_MODE)),
                GetInputPortValue<bool>(this.GetInputPortByName(IN_PORT_BOOL))
            );
        }
    }
}
