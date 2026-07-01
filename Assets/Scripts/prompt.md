Saya sedang mengembangkan game 2D Metroidvania menggunakan Unity 2022 LTS.

Sebelum mulai membuat kode, pahami terlebih dahulu seluruh requirement berikut dan ikuti semuanya tanpa mengubah arsitektur proyek.

==================================================================
STRUKTUR PROJECT
==================================================================

Semua script Player hanya boleh berada di dalam folder:

Assets/Scripts/Player/

Semua script Enemy hanya boleh berada di dalam folder:

Assets/Scripts/Enemy/

Jangan membuat folder baru selain kedua folder tersebut.

Jangan membuat sistem yang terlalu kompleks seperti Behavior Tree, GOAP, NavMesh, ECS, ataupun framework AI lain.

Gunakan State Machine sederhana berbasis Enum.

Semua kode harus mudah dibaca, diberi komentar seperlunya, dan mengikuti Single Responsibility Principle.

==================================================================
LAYER YANG DIGUNAKAN
==================================================================

Ground
Player
Enemy
PlayerHitbox
EnemyHitbox
PlayerHurtbox
EnemyHurtbox
Interactable

Ground digunakan untuk seluruh objek statis seperti:

- tanah
- platform
- dinding
- tebing

Tidak ada layer Obstacle.

==================================================================
PLAYER
==================================================================

Tambahkan sistem stat player yang terpisah dari movement.

Movement player sudah ada sehingga jangan diubah kecuali benar-benar diperlukan.

Buat sistem berikut:

PlayerStats

Berisi:

- Max HP
- Current HP
- Attack Damage
- Invincible Duration setelah terkena hit

PlayerHealth

Mengatur:

- menerima damage
- mengurangi HP
- mati ketika HP habis
- invincible frame
- memanggil animasi Hit
- memanggil animasi Death jika nanti tersedia

PlayerCombat

Mengatur:

- Hitbox serangan
- Mengaktifkan hitbox melalui Animation Event
- Menonaktifkan hitbox setelah animasi selesai

PlayerHurtbox

Menerima damage dari EnemyHitbox.

Player menerima damage ketika Hurtbox bersentuhan dengan EnemyHitbox.

Tambahkan Knockback sederhana.

PlayerAnimation

Mengontrol parameter animator yang berhubungan dengan Hit dan Death tanpa mengubah sistem movement yang sudah ada.

==================================================================
ENEMY
==================================================================

Enemy merupakan musuh darat.

Musuh tidak dapat melompat.

Musuh hanya bergerak horizontal.

Gunakan State Machine dengan state berikut:

Idle
Patrol
Chase
Hit
Dead

======================================================
IDLE
======================================================

Musuh diam selama beberapa detik.

Setelah timer selesai masuk Patrol.

======================================================
PATROL
======================================================

Musuh berjalan dengan Walk Speed.

Arah patrol mengikuti Facing Direction.

Sesekali berhenti lalu berjalan kembali agar terlihat hidup.

======================================================
CHASE
======================================================

Jika Player terlihat:

Musuh mengejar Player menggunakan Chase Speed.

Chase Speed lebih cepat daripada Walk Speed.

Musuh akan selalu menghadap Player.

======================================================
PLAYER DETECTION
======================================================

Gunakan dua tahap deteksi.

Tahap pertama:

Cek jarak Player.

Jika Player berada di luar Vision Range maka tidak perlu Raycast.

Tahap kedua:

Jika Player berada dalam Vision Range lakukan Raycast.

Raycast hanya boleh mengenai:

Ground
Player

Jika Raycast mengenai Ground terlebih dahulu maka Player tidak terlihat.

Player juga dianggap hilang apabila:

- keluar Vision Range
- tertutup Ground
- memiliki perbedaan ketinggian melebihi batas tertentu

Gunakan Max Height Difference agar Player dapat kabur ketika naik platform tinggi atau turun ke bawah.

======================================================
MEMORY SYSTEM
======================================================

Saat kehilangan Player jangan langsung kembali Patrol.

Gunakan Memory Timer.

Musuh tetap berjalan menuju posisi terakhir Player selama beberapa detik.

Jika Player tetap tidak ditemukan baru kembali Idle atau Patrol.

======================================================
GROUND CHECK
======================================================

Gunakan Ground Check.

Raycast kecil ke bawah di depan kaki musuh.

Jika tidak ada Ground maka musuh harus Flip.

Musuh tidak boleh jatuh dari platform.

======================================================
WALL CHECK
======================================================

Gunakan Raycast ke depan.

Jika mengenai Ground yang berada tepat di depan maka musuh harus Flip.

======================================================
HIT
======================================================

Saat menerima damage:

- AI berhenti sebentar
- memainkan animasi Hit
- menerima Knockback kecil
- kembali ke state sebelumnya apabila masih hidup

======================================================
DEAD
======================================================

Jika HP <= 0:

Masuk state Dead.

- hentikan AI
- hentikan movement
- nonaktifkan collider
- mainkan animasi Death
- Destroy GameObject setelah animasi selesai

======================================================
ANIMATION
======================================================

Animasi yang tersedia:

Enemy:

Idle
Run
Hit
Death

Run digunakan baik untuk Patrol maupun Chase.

Yang membedakan hanya kecepatan movement.

Belum ada animasi attack.

Enemy memberikan damage ketika EnemyHitbox bersentuhan dengan PlayerHurtbox.

======================================================
SCRIPT YANG DIHARAPKAN
======================================================

Folder Player

- PlayerStats.cs
- PlayerHealth.cs
- PlayerCombat.cs
- PlayerHurtbox.cs
- PlayerAnimation.cs

Folder Enemy

- EnemyStats.cs
- EnemyStateMachine.cs
- EnemyMovement.cs
- EnemyDetection.cs
- EnemyHealth.cs
- EnemyAnimation.cs

Boleh menambahkan script helper apabila benar-benar diperlukan, namun tetap berada pada folder Player atau Enemy.

======================================================
INSPECTOR
======================================================

Semua angka harus dapat diubah melalui Inspector.

Jangan menggunakan angka hardcode.

Contoh:

Walk Speed
Chase Speed
Vision Range
Memory Time
Ground Check Distance
Wall Check Distance
Knockback Force
Max HP
Attack Damage
Invincible Duration
Max Height Difference

======================================================
KODE
======================================================

Gunakan Rigidbody2D.

Gunakan Physics2D.

Gunakan Raycast2D.

Jangan menggunakan NavMesh.

Jangan menggunakan coroutine yang tidak diperlukan.

Usahakan Update hanya digunakan untuk pengecekan state.

Movement dilakukan pada FixedUpdate apabila berkaitan dengan Rigidbody2D.

Seluruh kode harus modular, mudah dibaca, mudah dikembangkan untuk musuh baru di masa depan, dan kompatibel dengan Unity 2022 LTS.