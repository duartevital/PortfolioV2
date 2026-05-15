<script setup lang="ts">
const props = defineProps<{
  src: string
  alt: string
  aspectRatio?: number   // height / width — drives placeholder sizing
}>()

const loaded = ref(false)
const imgRef = ref<HTMLImageElement | null>(null)

onMounted(() => {
  if (imgRef.value?.complete) loaded.value = true
})
</script>

<template>
  <div
    class="relative overflow-hidden bg-surface"
    :style="aspectRatio ? `padding-bottom: ${aspectRatio * 100}%` : undefined"
  >
    <!-- blur-up placeholder shimmer -->
    <div
      v-if="!loaded"
      class="absolute inset-0 animate-pulse bg-gradient-to-br from-surface via-border to-surface"
    />

    <img
      ref="imgRef"
      :src="src"
      :alt="alt"
      loading="lazy"
      decoding="async"
      class="absolute inset-0 w-full h-full object-cover transition-opacity duration-500"
      :class="loaded ? 'opacity-100' : 'opacity-0'"
      @load="loaded = true"
    />
  </div>
</template>
