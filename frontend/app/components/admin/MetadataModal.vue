<script setup lang="ts">
import type { Photo, PhotoCategory } from '~/types/photo'

const props = defineProps<{
  photo: Partial<Photo> & { _file?: File }
  isNew?: boolean
}>()

const emit = defineEmits<{
  save: [data: Partial<Photo>]
  cancel: []
}>()

const form = reactive({
  title:       props.photo.title       ?? '',
  category:    (props.photo.category   ?? 'landscape-nature') as PhotoCategory,
  description: props.photo.description ?? '',
  shootDate:   props.photo.shootDate   ?? new Date().toISOString().slice(0, 10),
  visible:     props.photo.visible     ?? true,
})

const categories: { label: string; value: PhotoCategory }[] = [
  { label: 'Landscape / Nature', value: 'landscape-nature' },
  { label: 'Street / Urban',     value: 'street-urban' },
]

function submit() {
  emit('save', { ...form })
}
</script>

<template>
  <div
    class="fixed inset-0 z-50 flex items-center justify-center bg-black/80 px-4"
    @click.self="emit('cancel')"
  >
    <form
      class="bg-surface border border-border rounded-lg w-full max-w-md p-6 space-y-4"
      @submit.prevent="submit"
    >
      <h2 class="font-serif text-lg text-text">{{ isNew ? 'Upload photo' : 'Edit metadata' }}</h2>

      <div class="space-y-3">
        <label class="block">
          <span class="text-muted text-xs font-sans uppercase tracking-wider">Title</span>
          <input
            v-model="form.title"
            required
            class="mt-1 w-full bg-bg border border-border rounded px-3 py-2 text-text font-sans text-sm focus:outline-none focus:border-accent transition-colors"
          />
        </label>

        <label class="block">
          <span class="text-muted text-xs font-sans uppercase tracking-wider">Category</span>
          <select
            v-model="form.category"
            class="mt-1 w-full bg-bg border border-border rounded px-3 py-2 text-text font-sans text-sm focus:outline-none focus:border-accent transition-colors"
          >
            <option v-for="c in categories" :key="c.value" :value="c.value">{{ c.label }}</option>
          </select>
        </label>

        <label class="block">
          <span class="text-muted text-xs font-sans uppercase tracking-wider">Description</span>
          <textarea
            v-model="form.description"
            rows="3"
            class="mt-1 w-full bg-bg border border-border rounded px-3 py-2 text-text font-sans text-sm focus:outline-none focus:border-accent transition-colors resize-none"
          />
        </label>

        <label class="block">
          <span class="text-muted text-xs font-sans uppercase tracking-wider">Shoot date</span>
          <input
            v-model="form.shootDate"
            type="date"
            required
            class="mt-1 w-full bg-bg border border-border rounded px-3 py-2 text-text font-sans text-sm focus:outline-none focus:border-accent transition-colors"
          />
        </label>

        <label class="flex items-center gap-3 cursor-pointer">
          <input v-model="form.visible" type="checkbox" class="accent-accent w-4 h-4" />
          <span class="text-muted text-sm font-sans">Visible in gallery</span>
        </label>
      </div>

      <div class="flex gap-3 pt-2">
        <button
          type="submit"
          class="flex-1 bg-accent hover:bg-accent-dim text-bg font-sans font-medium text-sm py-2 rounded transition-colors"
        >
          {{ isNew ? 'Upload' : 'Save' }}
        </button>
        <button
          type="button"
          class="flex-1 border border-border text-muted hover:text-text font-sans text-sm py-2 rounded transition-colors"
          @click="emit('cancel')"
        >
          Cancel
        </button>
      </div>
    </form>
  </div>
</template>
