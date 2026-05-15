<script setup lang="ts">
import type { Photo } from '~/types/photo'

definePageMeta({ middleware: 'auth', layout: false })

const { authHeaders, logout } = useAuth()
const config = useRuntimeConfig()
const baseUrl = config.public.apiBaseUrl || 'http://localhost:5000'

// ── state ─────────────────────────────────────────────────────────────────
const photos     = ref<Photo[]>([])
const loading    = ref(true)
const uploading  = ref(false)
const error      = ref<string | null>(null)

// modal state
const pendingFile    = ref<File | null>(null)
const editingPhoto   = ref<Photo | null>(null)
const deleteTarget   = ref<string | null>(null)

// ── fetch ──────────────────────────────────────────────────────────────────
async function fetchPhotos() {
  loading.value = true
  try {
    photos.value = await $fetch<Photo[]>(`${baseUrl}/api/v1/admin/photos`, {
      headers: authHeaders(),
    })
  } catch {
    error.value = 'Failed to load photos'
  } finally {
    loading.value = false
  }
}

onMounted(fetchPhotos)

// ── upload ─────────────────────────────────────────────────────────────────
function onFiles(files: FileList) {
  // queue first file; batch: loop through in a future iteration
  pendingFile.value = files[0]!
}

async function onUploadSave(meta: Partial<Photo>) {
  if (!pendingFile.value) return
  uploading.value = true
  error.value = null

  const form = new FormData()
  form.append('file',        pendingFile.value)
  form.append('title',       meta.title       ?? '')
  form.append('category',    meta.category    ?? 'landscape-nature')
  form.append('description', meta.description ?? '')
  form.append('shootDate',   meta.shootDate   ?? new Date().toISOString().slice(0, 10))
  form.append('visible',     String(meta.visible ?? true))

  try {
    await $fetch(`${baseUrl}/api/v1/admin/photos`, {
      method: 'POST',
      headers: authHeaders(),
      body: form,
    })
    await fetchPhotos()
  } catch {
    error.value = 'Upload failed'
  } finally {
    uploading.value = false
    pendingFile.value = null
  }
}

// ── edit ───────────────────────────────────────────────────────────────────
async function onEditSave(meta: Partial<Photo>) {
  if (!editingPhoto.value) return
  try {
    await $fetch(`${baseUrl}/api/v1/admin/photos/${editingPhoto.value.id}`, {
      method: 'PATCH',
      headers: authHeaders(),
      body: meta,
    })
    await fetchPhotos()
  } catch {
    error.value = 'Update failed'
  } finally {
    editingPhoto.value = null
  }
}

// ── visibility toggle ──────────────────────────────────────────────────────
async function toggleVisible(photo: Photo) {
  try {
    await $fetch(`${baseUrl}/api/v1/admin/photos/${photo.id}`, {
      method: 'PATCH',
      headers: authHeaders(),
      body: { visible: !photo.visible },
    })
    await fetchPhotos()
  } catch {
    error.value = 'Update failed'
  }
}

// ── delete ─────────────────────────────────────────────────────────────────
async function confirmDelete() {
  if (!deleteTarget.value) return
  try {
    await $fetch(`${baseUrl}/api/v1/admin/photos/${deleteTarget.value}`, {
      method: 'DELETE',
      headers: authHeaders(),
    })
    await fetchPhotos()
  } catch {
    error.value = 'Delete failed'
  } finally {
    deleteTarget.value = null
  }
}

// ── reorder ────────────────────────────────────────────────────────────────
async function move(index: number, direction: -1 | 1) {
  const arr = [...photos.value]
  const swapIndex = index + direction
  ;[arr[index], arr[swapIndex]] = [arr[swapIndex]!, arr[index]!]
  photos.value = arr

  try {
    await $fetch(`${baseUrl}/api/v1/admin/photos/reorder`, {
      method: 'PUT',
      headers: authHeaders(),
      body: { ids: arr.map(p => p.id) },
    })
  } catch {
    error.value = 'Reorder failed'
    await fetchPhotos()
  }
}
</script>

<template>
  <div class="min-h-screen bg-bg text-text font-sans">
    <!-- header -->
    <header class="flex items-center justify-between px-6 py-4 border-b border-border">
      <div>
        <h1 class="font-serif text-xl text-text">Admin Panel</h1>
        <p class="text-muted text-xs mt-0.5">Vital Photography</p>
      </div>
      <div class="flex items-center gap-4">
        <NuxtLink to="/" class="text-muted text-sm hover:text-text transition-colors">← Gallery</NuxtLink>
        <button class="text-muted text-sm hover:text-red-400 transition-colors" @click="logout">Sign out</button>
      </div>
    </header>

    <main class="max-w-2xl mx-auto px-6 py-8 space-y-8">
      <!-- error banner -->
      <div v-if="error" class="bg-red-900/30 border border-red-700 text-red-300 rounded px-4 py-3 text-sm flex justify-between">
        {{ error }}
        <button class="hover:text-white" @click="error = null">✕</button>
      </div>

      <!-- upload zone -->
      <section>
        <h2 class="text-muted text-xs uppercase tracking-widest mb-3">Upload</h2>
        <UploadZone @files="onFiles" />
        <p v-if="uploading" class="text-muted text-sm mt-2">Uploading and resizing…</p>
      </section>

      <!-- photo list -->
      <section>
        <h2 class="text-muted text-xs uppercase tracking-widest mb-3">
          Photos ({{ photos.length }})
        </h2>

        <div v-if="loading" class="text-muted text-sm">Loading…</div>

        <div v-else-if="photos.length === 0" class="text-muted text-sm">
          No photos yet. Upload one above.
        </div>

        <div v-else class="space-y-2">
          <PhotoAdminCard
            v-for="(photo, i) in photos"
            :key="photo.id"
            :photo="photo"
            :index="i"
            :total="photos.length"
            @edit="editingPhoto = $event"
            @delete="deleteTarget = $event"
            @toggle-visible="toggleVisible"
            @move-up="move($event, -1)"
            @move-down="move($event, 1)"
          />
        </div>
      </section>
    </main>

    <!-- upload metadata modal -->
    <MetadataModal
      v-if="pendingFile"
      :photo="{ title: pendingFile.name.replace(/\.[^.]+$/, '') }"
      :is-new="true"
      @save="onUploadSave"
      @cancel="pendingFile = null"
    />

    <!-- edit metadata modal -->
    <MetadataModal
      v-if="editingPhoto"
      :photo="editingPhoto"
      @save="onEditSave"
      @cancel="editingPhoto = null"
    />

    <!-- delete confirm -->
    <div
      v-if="deleteTarget"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/80 px-4"
      @click.self="deleteTarget = null"
    >
      <div class="bg-surface border border-border rounded-lg p-6 max-w-sm w-full space-y-4 text-center">
        <p class="text-text font-sans text-sm">Delete this photo permanently?</p>
        <div class="flex gap-3">
          <button
            class="flex-1 bg-red-700 hover:bg-red-600 text-white font-sans text-sm py-2 rounded transition-colors"
            @click="confirmDelete"
          >
            Delete
          </button>
          <button
            class="flex-1 border border-border text-muted hover:text-text font-sans text-sm py-2 rounded transition-colors"
            @click="deleteTarget = null"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
