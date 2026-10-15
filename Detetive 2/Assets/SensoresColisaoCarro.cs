using UnityEngine;
using System.Collections.Generic;

public class SensoresColisaoCarro : MonoBehaviour
{
    [Header("========== SENSORES ==========")]

    public Collider colliderFrente;
    public Collider colliderTras;
    public Collider colliderEsquerda;
    public Collider colliderDireita;

    [Header("========== IGNORAR ==========")]

    public bool ignorarCollidersDoProprioCarro = true;

    [Header("========== ESTADO ==========")]

    public bool frenteBloqueada;
    public bool trasBloqueado;
    public bool esquerdaBloqueada;
    public bool direitaBloqueada;

    private HashSet<Collider> colisoresFrente =
        new HashSet<Collider>();

    private HashSet<Collider> colisoresTras =
        new HashSet<Collider>();

    private HashSet<Collider> colisoresEsquerda =
        new HashSet<Collider>();

    private HashSet<Collider> colisoresDireita =
        new HashSet<Collider>();

    void Awake()
    {
        ConfigurarTrigger(colliderFrente);
        ConfigurarTrigger(colliderTras);
        ConfigurarTrigger(colliderEsquerda);
        ConfigurarTrigger(colliderDireita);
    }

    void FixedUpdate()
    {
        LimparColisoresDestruidos();

        frenteBloqueada =
            colisoresFrente.Count > 0;

        trasBloqueado =
            colisoresTras.Count > 0;

        esquerdaBloqueada =
            colisoresEsquerda.Count > 0;

        direitaBloqueada =
            colisoresDireita.Count > 0;
    }

    void ConfigurarTrigger(Collider collider)
    {
        if (collider == null)
            return;

        collider.isTrigger = true;
    }

    bool DeveIgnorar(Collider outro)
    {
        if (!ignorarCollidersDoProprioCarro)
            return false;

        if (outro == null)
            return true;

        if (outro.transform.IsChildOf(transform))
            return true;

        if (transform.IsChildOf(outro.transform))
            return true;

        return false;
    }

    void OnTriggerEnter(Collider outro)
    {
        ProcessarEntrada(outro);
    }

    void OnTriggerStay(Collider outro)
    {
        ProcessarEntrada(outro);
    }

    void OnTriggerExit(Collider outro)
    {
        if (outro == null)
            return;

        colisoresFrente.Remove(outro);
        colisoresTras.Remove(outro);
        colisoresEsquerda.Remove(outro);
        colisoresDireita.Remove(outro);
    }

    void ProcessarEntrada(Collider outro)
    {
        if (DeveIgnorar(outro))
            return;

        if (colliderFrente != null &&
            colliderFrente.bounds.Intersects(outro.bounds))
        {
            colisoresFrente.Add(outro);
        }

        if (colliderTras != null &&
            colliderTras.bounds.Intersects(outro.bounds))
        {
            colisoresTras.Add(outro);
        }

        if (colliderEsquerda != null &&
            colliderEsquerda.bounds.Intersects(outro.bounds))
        {
            colisoresEsquerda.Add(outro);
        }

        if (colliderDireita != null &&
            colliderDireita.bounds.Intersects(outro.bounds))
        {
            colisoresDireita.Add(outro);
        }
    }

    void LimparColisoresDestruidos()
    {
        colisoresFrente.RemoveWhere(c => c == null);
        colisoresTras.RemoveWhere(c => c == null);
        colisoresEsquerda.RemoveWhere(c => c == null);
        colisoresDireita.RemoveWhere(c => c == null);
    }

    public bool PodeIrParaFrente()
    {
        return !frenteBloqueada;
    }

    public bool PodeDarRe()
    {
        return !trasBloqueado;
    }

    public bool PodeVirarEsquerda()
    {
        return !esquerdaBloqueada;
    }

    public bool PodeVirarDireita()
    {
        return !direitaBloqueada;
    }

    public bool PodeVirar(float direcao)
    {
        if (direcao < 0f)
            return PodeVirarEsquerda();

        if (direcao > 0f)
            return PodeVirarDireita();

        return true;
    }

    void OnDrawGizmos()
    {
        DesenharSensor(colliderFrente, Color.red);
        DesenharSensor(colliderTras, Color.blue);
        DesenharSensor(colliderEsquerda, Color.green);
        DesenharSensor(colliderDireita, Color.yellow);
    }

    void DesenharSensor(Collider sensor, Color cor)
    {
        if (sensor == null)
            return;

        Gizmos.color = cor;

        Bounds bounds = sensor.bounds;

        Gizmos.DrawWireCube(
            bounds.center,
            bounds.size
        );
    }
}