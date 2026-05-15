<script setup lang="ts">
import type { Photo } from '~/types/photo'

const props = defineProps<{
  photos: Photo[]
}>()

const emit = defineEmits<{
  open: [photo: Photo, index: number]
}>()

// Extract width/height from picsum URLs like /seed/1/400/560
// Falls back to a neutral 3:4 aspect ratio for real blob URLs
function aspectRatio(url: string): number {
  const m = url.match(/\/(\d+)\/(\d+)\s*$/)
  if (m) return Number(m[2]) / Number(m[1])
  return 4 / 3
}
</script>

<template>
  <div class="masonry-grid px-4 md:px-8 pb-16">
    <button
      v-for="(photo, i) in photos"
      :key="photo.id"
      class="masonry-item group relative overflow-hidden rounded-sm cursor-pointer focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-accent"
      :aria-label="`Open ${photo.title}`"
      @click="emit('open', photo, i)"
    >
      <BlurImage
        :src="photo.thumbnailUrl"
        :alt="photo.title"
        :aspect-ratio="aspectRatio(photo.thumbnailUrl)"
      />

      <!-- hover overlay -->
      <div class="absolute inset-0 bg-black/0 group-hover:bg-black/40 transition-colors duration-300 flex items-end p-4 pointer-events-none">
        <p class="text-text font-sans text-sm font-light translate-y-2 opacity-0 group-hover:translate-y-0 group-hover:opacity-100 transition-all duration-300">
          {{ photo.title }}
        </p>
      </div>
    </button>
  </div>
</template>

<style scoped>
.masonry-grid {
  columns: 2;
  column-gap: 0.75rem;
}

@media (min-width: 768px) {
  .masonry-grid { columns: 3; column-gap: 1rem; }
}

@media (min-width: 1280px) {
  .masonry-grid { columns: 4; column-gap: 1rem; }
}

.masonry-item {
  break-inside: avoid;
  display: block;
  width: 100%;
  margin-bottom: 0.75rem;
}

@media (min-width: 768px) {
  .masonry-item { margin-bottom: 1rem; }
}
</style>
