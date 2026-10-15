using UnityEngine;
using System.Collections.Generic;

public class SensorDirecaoCarro : MonoBehaviour
{
    public enum DirecaoSensor
    {
        Frente,
        Tras,
        Esquerda,
        Direita
    }

    [Header("Direção deste sensor")]
    public DirecaoSensor direcao;

    private HashSet<Collider> objetosDentro =
        new HashSet<Collider>();

    public bool Bloqueado
    {
        get
        {
            LimparObjetosInvalidos();
            return objetosDentro.Count > 0;
        }
    }

    private void Awake()
    {
        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider outro)
    {
        if (outro == null)
            return;

        if (outro.transform.IsChildOf(transform.root))
            return;

        objetosDentro.Add(outro);
    }

    private void OnTriggerStay(Collider outro)
    {
        if (outro == null)
            return;

        if (outro.transform.IsChildOf(transform.root))
            return;

        objetosDentro.Add(outro);
    }

    private void OnTriggerExit(Collider outro)
    {
        if (outro == null)
            return;

        objetosDentro.Remove(outro);
    }

    private void LimparObjetosInvalidos()
    {
        objetosDentro.RemoveWhere(
            collider => collider == null
        );
    }
}