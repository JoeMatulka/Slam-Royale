public class StateManager {
    private ActorState m_state = ActorState.DEFAULT;

    public delegate void StateEventHandler(object sender, StateEventArgs e);
    private event StateEventHandler m_stateEvent;

    public ActorState UpdateState(ActorState state) {
        if (m_state.Equals(ActorState.RECOVERING) &&
            state.Equals(ActorState.ATTACKING)) {
            return m_state;
        }
        m_state = state;

        // Notify listeners of state change
        StateEventArgs e = new StateEventArgs(m_state);
        StateEventHandler handler = StateEvent;
        if (handler != null)
        {
            handler(this, e);
        }

        return m_state;
    }

    public ActorState State {
        get { return m_state; }
    }

    public StateEventHandler StateEvent {
        get { return m_stateEvent; }
    }
}
