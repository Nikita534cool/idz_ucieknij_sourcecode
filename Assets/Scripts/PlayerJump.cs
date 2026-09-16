using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 5f; // Si�a skoku
    public LayerMask groundLayer; // Warstwa "ziemi" (platform)

    private Rigidbody2D rb;
    private int groundContactCount; // Liczba kolizji z ziemi� (zamiast pojedynczej flagi)
    private bool isGrounded => groundContactCount > 0; // Gracz jest na ziemi, je�li ma co najmniej jeden kontakt

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Brak komponentu Rigidbody2D na tym obiekcie! Dodaj Rigidbody2D, aby skrypt dzia�a� poprawnie.");
        }
    }

    void Update()
    {
        // Je�li lewy przycisk myszy zosta� klikni�ty i gracz jest na ziemi, wykonaj skok
        if (rb != null && Input.GetMouseButtonDown(0) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Resetuj pionow� pr�dko��, aby skok by� sp�jny
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // Sprawdza, czy dowolny punkt kontaktu kolizji wskazuje, �e gracz stoi na obiekcie od g�ry
    private bool HasGroundContactFromBelow(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Vector2.Dot(contact.normal, Vector2.up) > 0.5f) // U�yj warto�ci np. 0.5f jako pr�g
            {
                return true;
            }
        }
        return false;
    }

    // Metoda wywo�ywana, gdy ten kolider wejdzie w kolizj� z innym koliderem
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Sprawd�, czy obiekt, z kt�rym nast�pi�a kolizja, jest na warstwie "ziemi"
        if (((1 << collision.gameObject.layer) & groundLayer) != 0 && HasGroundContactFromBelow(collision))
        {
            groundContactCount++;
        }
    }

    // Metoda wywo�ywana, gdy ten kolider przestanie kolidowa� z innym koliderem
    void OnCollisionExit2D(Collision2D collision)
    {
        // Sprawd�, czy obiekt, z kt�rym przestali�my kolidowa�, by� na warstwie "ziemi".
        // U�ywamy licznika kontakt�w zamiast pojedynczej flagi, aby gracz pozosta� "uziemiony",
        // je�li nadal dotyka innej platformy (np. stoj�c na granicy dw�ch kolider�w).
        if (((1 << collision.gameObject.layer) & groundLayer) != 0 && groundContactCount > 0)
        {
            groundContactCount--;
        }
    }
}