using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManagerScript : MonoBehaviour
{
    [SerializeField]
    private bool raycast;

    [SerializeField]
    private float range;

    [SerializeField]
    private int damage;

    //prefab du laser
    [SerializeField]
    private GameObject bulletPrefab;

    //tableau de l'ensemble des laser déjà instantiés
    private BulletExposerScript[] alreadyInstanciatedBullets;

    //queue des laser disponibles
    private readonly Queue<BulletExposerScript> availableBullets = new Queue<BulletExposerScript>(100);

    //liste des lasers tirés
    private readonly List<BulletExposerScript> poppedBullets = new List<BulletExposerScript>(100);

    private bool firing = false;

    //au chargement desjoueurs
    public void Awake()
    {
        if (raycast) return;

        GameObject instanciatedBullet;

        //on génère 100 lasers
        alreadyInstanciatedBullets = new BulletExposerScript[100];

        for (int i = 0; i < 100; i++)
        {
            //instanciation du laser et ajout de son exposer au tableau des lasers instanciés
            instanciatedBullet = (GameObject)Instantiate(bulletPrefab);
            alreadyInstanciatedBullets[i] = instanciatedBullet.GetComponent<BulletExposerScript>();
        }

        //on met ensuite tous ces lasers dans la queue des lasers disponibles
        foreach (BulletExposerScript bullet in alreadyInstanciatedBullets)
        {
            availableBullets.Enqueue(bullet);
        }
    }

    public bool isRaycast()
    {
        return raycast;
    }

    public void SetFiring(bool f)
    {
        firing = f;
    }

    //fonction pour prendre un laser dans le pooling
    public BulletExposerScript GetBullet()
    {
        //on sort un laser de la queue
        BulletExposerScript bullet = availableBullets.Dequeue();

        //activation du laser
        bullet.Enable();

        return bullet;
    }

    //fonction pour remettre un laser dans le pooling
    public void ReleaseBullet(BulletExposerScript bullet)
    {
        //ajout à la queue et désactivation du laser
        availableBullets.Enqueue(bullet);
        bullet.Disable();
    }

    public void ShootWithRaycast(Transform popPosition)
    {
        RaycastHit hit;

        if (Physics.Raycast(popPosition.position, popPosition.forward, out hit, range)
            && Equals(hit.transform.gameObject.tag, "Player")) {
            ShipExposer target = hit.transform.GetComponent<ShipExposer>();

            if (target != null)
                target.TakeDamage(damage);
        }
    }

    //fonction pour pop un laser à la position indiquée
    public void Shoot(Transform popPosition)
    {
        if (raycast) {
            ShootWithRaycast(popPosition);
            return;
        }
        //on récupère un laser
        BulletExposerScript bullet = GetBullet();

        //on lui donne les caractéristiques de la source
        bullet.SetParentReference(
            popPosition.position,
            popPosition.forward,
            popPosition.rotation
        );

        //ajout du laser à la liste des lasers tirés
        poppedBullets.Add(bullet);
    }

    void FixedUpdate()
    {
        if (raycast && firing) {}

        //pour chaque laser tiré
        for (int i = 0; i < poppedBullets.Count; i++)
        {
            BulletExposerScript bullet = poppedBullets[i];

            //si le laser se fait détruire, on le sort du tableau des laser tirés pour le remettre dans la queue des laser dispo
			if (bullet.GetDestroy())
            {
                poppedBullets.RemoveAt(i--);
                ReleaseBullet(bullet);
            }
        }
    }
}
