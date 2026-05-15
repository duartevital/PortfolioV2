<script setup lang="ts">
import type { Photo } from '~/types/photo'

const props = defineProps<{
  photo: Photo
  index: number
  total: number
}>()

const emit = defineEmits<{
  edit:       [photo: Photo]
  delete:     [id: string]
  toggleVisible: [photo: Photo]
  moveUp:     [index: number]
  moveDown:   [index: number]
}>()

const categoryLabel: Record<string, string> = {
  'landscape-nature': 'Landscape',
  'street-urban':     'Street',
}
</script>

<template>
  <div class="flex gap-3 p-3 bg-surface border border-border rounded-lg items-start">
    <!-- thumbnail -->
    <img
      :src="photo.thumbnailUrl"
      :alt="photo.title"
      class="w-16 h-16 object-cover rounded flex-shrink-0"
    />

    <!-- info -->
    <div class="flex-1 min-w-0">
      <p class="text-text font-sans text-sm font-medium truncate">{{ photo.title }}</p>
      <p class="text-muted font-sans text-xs mt-0.5">
        {{ categoryLabel[photo.category] }} · {{ photo.shootDate }}
      </p>
      <span
        class="inline-block mt-1 text-xs font-sans px-2 py-0.5 rounded-full"
        :class="photo.visible ? 'bg-accent/20 text-accent' : 'bg-border text-muted'"
      >
        {{ photo.visible ? 'Visible' : 'Hidden' }}
      </span>
    </div>

    <!-- actions -->
    <div class="flex flex-col gap-1 flex-shrink-0">
      <!-- reorder -->
      <div class="flex gap-1">
        <button
          :disabled="index === 0"
          class="text-muted hover:text-text disabled:opacity-30 text-xs px-1"
          aria-label="Move up"
          @click="emit('moveUp', index)"
        >↑</button>
        <button
          :disabled="index === total - 1"
          class="text-muted hover:text-text disabled:opacity-30 text-xs px-1"
          aria-label="Move down"
          @click="emit('moveDown', index)"
        >↓</button>
      </div>

      <button
        class="text-muted hover:text-text text-xs font-sans"
        @click="emit('toggleVisible', photo)"
      >
        {{ photo.visible ? 'Hide' : 'Show' }}
      </button>
      <button
        class="text-muted hover:text-text text-xs font-sans"
        @click="emit('edit', photo)"
      >
        Edit
      </button>
      <button
        class="text-red-400 hover:text-red-300 text-xs font-sans"
        @click="emit('delete', photo.id)"
      >
        Delete
      </button>
    </div>
  </div>
</template>
