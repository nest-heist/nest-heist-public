    public abstract class StateMachine
    {
        protected IState _currentState;

        public void ChangeState(IState state)
        {
            _currentState?.Exit();
            _currentState = state;
            _currentState?.Enter();
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void PhysicsUpdate()
        {
            _currentState?.PhysicsUpdate();
        }

        public void HandleInput()
        {
            _currentState?.HandleInput();
        }
    }
