<script setup lang="ts">
import type { Photo, PhotoCategory } from '~/types/photo'

type FilterValue = 'all' | PhotoCategory

const activeFilter = ref<FilterValue>('all')
const { photos } = useMockPhotos(activeFilter)

const lightboxOpen = ref(false)
const lightboxIndex = ref(0)

function openLightbox(photo: Photo, index: number) {
  lightboxIndex.value = index
  lightboxOpen.value = true
}
</script>

<template>
  <div>
    <FilterBar v-model="activeFilter" />
    <MasonryGrid :photos="photos" @open="openLightbox" />
    <Lightbox
      v-if="lightboxOpen"
      :photos="photos"
      :initial-index="lightboxIndex"
      @close="lightboxOpen = false"
    />
  </div>
</template>
