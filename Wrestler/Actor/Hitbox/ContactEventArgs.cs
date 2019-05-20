using System;
using UnityEngine;

public class ContactEventArgs : EventArgs {
    private Collider m_col;

    public ContactEventArgs(Collider col) {
        m_col = col;
    }

    public Collider Collider {
        get { return m_col; }
    } 
}
