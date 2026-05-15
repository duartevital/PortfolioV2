import type { Photo, PhotoCategory } from '~/types/photo'

export function usePhotos(category?: Ref<PhotoCategory | 'all'>) {
  const config = useRuntimeConfig()
  const baseUrl = import.meta.server
    ? (config.apiBaseUrl || config.public.apiBaseUrl || 'http://localhost:5000')
    : (config.public.apiBaseUrl || 'http://localhost:5000')

  const url = computed(() => {
    const cat = category?.value
    const params = cat && cat !== 'all' ? `?category=${cat}` : ''
    return `${baseUrl}/api/v1/photos${params}`
  })

  const { data, status, error, refresh } = useFetch<Photo[]>(url, {
    lazy: true,
    default: () => [],
  })

  return { photos: data, status, error, refresh }
}
