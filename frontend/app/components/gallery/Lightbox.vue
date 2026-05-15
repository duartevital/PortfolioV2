<script setup lang="ts">
import type { Photo } from '~/types/photo'

const props = defineProps<{
  photos: Photo[]
  initialIndex: number
}>()

const emit = defineEmits<{ close: [] }>()

const current = ref(props.initialIndex)
const photo = computed(() => props.photos[current.value]!)
const imgLoaded = ref(false)

watch(current, () => { imgLoaded.value = false })

function prev() { current.value = (current.value - 1 + props.photos.length) % props.photos.length }
function next() { current.value = (current.value + 1) % props.photos.length }

function onKey(e: KeyboardEvent) {
  if (e.key === 'ArrowLeft')  prev()
  if (e.key === 'ArrowRight') next()
  if (e.key === 'Escape')     emit('close')
}

onMounted(() => {
  document.addEventListener('keydown', onKey)
  document.body.style.overflow = 'hidden'
})
onUnmounted(() => {
  document.removeEventListener('keydown', onKey)
  document.body.style.overflow = ''
})
</script>

<template>
  <Teleport to="body">
    <div
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/95"
      role="dialog"
      aria-modal="true"
      :aria-label="photo.title"
      @click.self="emit('close')"
    >
      <!-- close -->
      <button
        class="absolute top-4 right-5 text-muted hover:text-text transition-colors text-3xl leading-none z-10"
        aria-label="Close"
        @click="emit('close')"
      >
        &times;
      </button>

      <!-- prev -->
      <button
        v-if="photos.length > 1"
        class="absolute left-3 md:left-6 text-muted hover:text-text transition-colors p-2 z-10 text-2xl"
        aria-label="Previous photo"
        @click="prev"
      >
        &#8592;
      </button>

      <!-- image -->
      <div class="flex flex-col items-center max-w-5xl w-full px-12 md:px-20">
        <div class="relative w-full flex items-center justify-center" style="max-height: 80vh">
          <div v-if="!imgLoaded" class="absolute inset-0 flex items-center justify-center">
            <div class="w-8 h-8 border-2 border-accent border-t-transparent rounded-full animate-spin" />
          </div>
          <img
            :key="photo.id"
            :src="photo.displayUrl"
            :alt="photo.title"
            class="max-h-[80vh] max-w-full object-contain transition-opacity duration-300"
            :class="imgLoaded ? 'opacity-100' : 'opacity-0'"
            @load="imgLoaded = true"
          />
        </div>

        <div class="mt-4 text-center">
          <p class="font-serif text-lg text-text">{{ photo.title }}</p>
          <p v-if="photo.shootDate" class="text-muted text-sm font-sans mt-1">{{ photo.shootDate }}</p>
        </div>

        <p class="text-border text-xs font-sans mt-3">
          {{ current + 1 }} / {{ photos.length }}
        </p>
      </div>

      <!-- next -->
      <button
        v-if="photos.length > 1"
        class="absolute right-3 md:right-6 text-muted hover:text-text transition-colors p-2 z-10 text-2xl"
        aria-label="Next photo"
        @click="next"
      >
        &#8594;
      </button>
    </div>
  </Teleport>
</template>
