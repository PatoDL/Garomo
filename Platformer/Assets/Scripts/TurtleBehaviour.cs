using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleBehaviour : MonoBehaviour
{
    public float velX;
    public Vector2 direction;
    Rigidbody2D rig;
    SpriteRenderer spriteRenderer;

    public float raycastOffset;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = -transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D[] raycastHit2D = new RaycastHit2D[2];

        raycastHit2D[0] = Physics2D.Raycast(new Vector3(transform.position.x - spriteRenderer.size.x / 2 * transform.localScale.x - raycastOffset, transform.position.y - raycastOffset, transform.position.z), -transform.up, 1);
        raycastHit2D[1] = Physics2D.Raycast(new Vector3(transform.position.x + spriteRenderer.size.x / 2 * transform.localScale.x + raycastOffset, transform.position.y - raycastOffset, transform.position.z), -transform.up, 1);

        if (!raycastHit2D[0] || !raycastHit2D[1])
        {
            direction *= -1;
            Vector3 flip = spriteRenderer.transform.localScale;
            flip.x *= -1;
            spriteRenderer.transform.localScale = flip;
        }

        //Debug.DrawRay(new Vector3(transform.position.x + spriteRenderer.size.x  * transform.localScale.x / 2 + raycastOffset, transform.position.y - raycastOffset, transform.position.z), -transform.up);
        //Debug.DrawRay(new Vector3(transform.position.x - spriteRenderer.size.x  * transform.localScale.x / 2 - raycastOffset, transform.position.y - raycastOffset, transform.position.z), -transform.up);
    }

    void FixedUpdate()
    {
        rig.velocity = new Vector3(velX * Time.fixedDeltaTime * direction.x, rig.velocity.y);
    }
}
