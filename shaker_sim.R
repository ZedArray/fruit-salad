# Shaker spawns/clears in MainScene, with clear time derived from the damage sources
# (StageUtensilSpawner + AntiIdleUtensilSpawner are the only things that damage shakers).
# Assumes both stages are active for the whole run. Every curve/number below is read from the scene/assets.

# ShakerHealth: Ramp 0.075n^2+20 on [0,20], then 0.5n + Ramp(19.9) extrapolated; rounded, min 1
health <- function(n) pmax(1, round(ifelse(n <= 20, 0.075 * n^2 + 20, 0.5 * n + 0.075 * 19.9^2 + 20)))

# Utensil BalancingCurve (CompositeCurve.asset) = seconds between utensil batches at time x (extrapolates, floor 0.4 in fast2)
slow1 <- function(x) 4 - x^0.2
fast1 <- function(x) slow1(7.8) + log(7.8, 13) - log(x, 13)
slow2 <- function(x) fast1(23) + 23^0.38 - x^0.38
fast2 <- function(x) pmax(0.4, slow2(40) + 40^0.23 - x^0.23)
interval <- function(x) pmax(0.016, ifelse(x < 7.8, slow1(x), ifelse(x < 23, fast1(x), ifelse(x < 40, slow2(x), fast2(x)))))

# StageUtensilSpawner: Random.Range(0, maxPerSpawn=2) is int-exclusive => 0|1, loop i>=0 => 1 or 2 arrows (mean 1.5), damage 5
# AntiIdleUtensilSpawner: 1 arrow per cooldown(1s) while idle >= 1s, damage 10
# Arrows are triggers that pass through, so every arrow whose path crosses a shaker hits it (0.2s i-frames ignored).
# pHit = fraction of arrows crossing a given shaker (depends on where the player stands); idleFrac = share of time the player is idle.
# Stage cycle (StageData.asset): None 3s once, then Blade 25 / Utensil 25 / Hot 20 repeating (70s).
# ShakerSpawner + AntiIdle are on in every stage; StageUtensilSpawner only in the UtensilStage.
inUtensil <- function(t) t >= 28 & ((t - 28) %% 70) < 25

# dmg/s on one landed shaker. Utensil batches only in UtensilStage; anti-idle arrows (10 dmg, 1/s) whenever the player idles.
# pHit = fraction of arrows whose path crosses that shaker (player positioning); idle = share of time idle.
dps <- function(t, pHit, idle)
  pHit * (inUtensil(t) * 1.5 * 5 / interval(t) + idle * 10)

landDelay <- 1.5   # warningTime 1s + ~0.5s fall
dt <- 0.05

# idleOther = idle share outside the UtensilStage (0 = "store": wait for the utensil stage; >0 = "clear ASAP")
# Spawn attempts every `gap` s from t=3 (mean of 10-20); spawn only if alive < 3. Health is set from the SPAWN index, not clears.
sim <- function(D, pHit, idleOther, idleUtensil = 0.5, gap = 15, maxShakers = 3) {
  tt <- seq(0, D, by = dt)
  attempts <- seq(3 + gap, D, by = gap); ai <- 1
  hp <- numeric(); land <- numeric(); born <- numeric(); n <- 0; clears <- 0; life <- numeric()
  for (t in tt) {
    while (ai <= length(attempts) && attempts[ai] <= t) {
      if (length(hp) < maxShakers) { hp <- c(hp, health(n)); land <- c(land, t + landDelay); born <- c(born, t); n <- n + 1 }
      ai <- ai + 1
    }
    if (length(hp)) {
      d <- dps(t, pHit, if (inUtensil(t)) idleUtensil else idleOther) * dt
      hp <- hp - d * (t >= land)
      dead <- hp <= 0
      if (any(dead)) { clears <- clears + sum(dead); life <- c(life, t - born[dead]); hp <- hp[!dead]; land <- land[!dead]; born <- born[!dead] }
    }
  }
  c(spawns = n, clears = clears, coins = 3 * clears, meanLife = if (length(life)) mean(life) else NA, lastHealth = health(n - 1))
}

if (sys.nframe() == 0) {
  D <- 420  # 6 stage cycles
  g <- expand.grid(pHit = c(0.5, 1), strategy = c("store (idle 0 outside utensil)", "ASAP (idle 0.5 outside)"))
  r <- t(mapply(function(p, s) sim(D, p, if (grepl("store", s)) 0 else 0.5), g$pHit, g$strategy))
  print(cbind(g, round(r, 1)))
  stopifnot(health(0) == 20, health(20) == 50, abs(interval(0) - 4) < 1e-9, inUtensil(30), !inUtensil(60))
}
