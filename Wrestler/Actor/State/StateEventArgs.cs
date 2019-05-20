using System;

public class StateEventArgs : EventArgs {
    private ActorState m_state;

    public StateEventArgs(ActorState state) {
        m_state = state;
    }

    public ActorState State {
        get { return m_state; }
    }
}
