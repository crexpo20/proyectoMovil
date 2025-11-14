using UnityEngine;

public class cajaloot : MonoBehaviour
{

    [SerializeField] private GameObject[] lootcaja;


    [SerializeField] private Vector2 fuerzaLootRango;
    [SerializeField] private float offsetYLoot;
    [SerializeField] private Vector2 offsetPosicionXLoot;

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Player"))
        {
            recolectar();
        }
    }
    private void recolectar()
    {
        generarLoot();
        Destroy(gameObject);
       
    }
    private void generarLoot() {
        foreach (GameObject objeto in lootcaja) {

            float posicionXAleatoria = Random.Range(offsetPosicionXLoot.x,
                offsetPosicionXLoot.y);

            Vector3 posicionCrearObjeto = new(
                transform.position.x + posicionXAleatoria,
                transform.position.y + offsetYLoot,
                transform.position.z);

            GameObject objetocreado = Instantiate(objeto, transform.position, Quaternion.identity);

            if (objetocreado.TryGetComponent(out Rigidbody2D rb2D))
            {
                Vector2 direccion = objetocreado.transform.position - transform.position;

                float fuerzaAleatoria = Random.Range(fuerzaLootRango.x, fuerzaLootRango.y);

                rb2D.AddForce(direccion * fuerzaAleatoria, ForceMode2D.Impulse);
            }
        }
    
    }
}
