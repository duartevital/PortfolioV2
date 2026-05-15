<script setup lang="ts">
const emit = defineEmits<{ files: [FileList] }>()

const dragging = ref(false)
const inputRef = ref<HTMLInputElement | null>(null)

function onDrop(e: DragEvent) {
  dragging.value = false
  const files = e.dataTransfer?.files
  if (files?.length) emit('files', files)
}

function onPick(e: Event) {
  const files = (e.target as HTMLInputElement).files
  if (files?.length) emit('files', files)
  if (inputRef.value) inputRef.value.value = ''
}
</script>

<template>
  <div
    class="relative border-2 border-dashed rounded-lg p-10 text-center transition-colors cursor-pointer"
    :class="dragging ? 'border-accent bg-accent/5' : 'border-border hover:border-accent/50'"
    @dragover.prevent="dragging = true"
    @dragleave="dragging = false"
    @drop.prevent="onDrop"
    @click="inputRef?.click()"
  >
    <input
      ref="inputRef"
      type="file"
      accept="image/*"
      multiple
      class="sr-only"
      @change="onPick"
    />
    <p class="text-muted font-sans text-sm">
      Drop photos here or <span class="text-accent">browse</span>
    </p>
    <p class="text-border font-sans text-xs mt-1">JPEG, PNG, WebP · up to 50 MB each</p>
  </div>
</template>
