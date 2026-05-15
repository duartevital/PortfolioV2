<template>
  <div class="max-w-5xl mx-auto px-6 py-16 md:py-24">

    <!-- intro line -->
    <p class="text-muted font-sans text-xs uppercase tracking-widest mb-12">About</p>

    <!-- two-column layout: portrait + bio -->
    <div class="grid md:grid-cols-[1fr_1.6fr] gap-12 md:gap-20 items-start">

      <!-- portrait -->
      <div class="relative">
        <div class="overflow-hidden rounded-sm aspect-[3/4] bg-surface">
          <!-- replace src with real portrait URL once uploaded via admin -->
          <img
            src="https://picsum.photos/seed/portrait/600/800"
            alt="Photographer portrait"
            class="w-full h-full object-cover"
            loading="lazy"
          />
        </div>
        <div class="absolute -bottom-3 -right-3 w-24 h-24 border border-accent/30 rounded-sm -z-10" />
      </div>

      <!-- bio -->
      <div class="space-y-10">
        <div>
          <h1 class="font-serif text-4xl md:text-5xl text-text leading-tight mb-6">
            <!-- replace with your name -->
            Duarte Vital
          </h1>
          <div class="space-y-4 text-muted font-sans text-base leading-relaxed">
            <!-- replace with real bio -->
            <p>
              I'm a hobbyist photographer based in Portugal, working across landscape, nature,
              and street photography. I've been shooting seriously for the past three years,
              drawn to the tension between stillness and motion — the moment just before
              or just after something happens.
            </p>
            <p>
              My work gravitates toward low light and quiet scenes: empty streets at dusk,
              mountain ridges at dawn, rain-slicked cobblestones. I'm interested in
              what a photograph withholds as much as what it shows.
            </p>
            <p>
              I'm currently transitioning into semi-professional work — available for
              commissioned projects, print licensing, and exhibition inquiries.
            </p>
          </div>
        </div>

        <!-- divider -->
        <div class="border-t border-border" />

        <!-- gear & philosophy -->
        <div class="grid sm:grid-cols-2 gap-8">
          <div>
            <h2 class="font-sans text-xs uppercase tracking-widest text-muted mb-4">Gear</h2>
            <ul class="space-y-2 font-sans text-sm text-muted">
              <!-- replace with your actual kit -->
              <li>Sony A7 IV</li>
              <li>Sony 24–70mm f/2.8 GM II</li>
              <li>Sony 85mm f/1.4 GM</li>
              <li>Voigtländer 21mm f/1.4</li>
            </ul>
          </div>

          <div>
            <h2 class="font-sans text-xs uppercase tracking-widest text-muted mb-4">Philosophy</h2>
            <p class="font-sans text-sm text-muted leading-relaxed">
              <!-- replace with your own words -->
              Shoot less, see more. I'd rather spend an hour in one place than chase
              ten different spots. The best light is the one you're already standing in.
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
